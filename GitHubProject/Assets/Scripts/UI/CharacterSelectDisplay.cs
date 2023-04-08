using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    //[SerializeField] private CharacterDatabaseClass characterDatabaseClass;
    [SerializeField] private Transform charactersHolder;
    [SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private PlayerCard[] playerCards;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Transform introSpawnPoint;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private Button lockInButton;

    private GameObject introInstance;
    private List<CharacterSelectButton> characterButtons = new List<CharacterSelectButton>();
    private Vector2 lastButtonPosition = new Vector2(0, 50);

    private NetworkList<CharacterSelectState> players;

    private void Awake() 
    {
        players = new NetworkList<CharacterSelectState>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            Character[] allCharacters = characterDatabase.GetAllCharacters();
            //CharacterClass[] allCharacters = characterDatabaseClass.GetAllCharacters();

            foreach (var character in allCharacters)
            {
                var selectButtonInstance = Instantiate(selectButtonPrefab, charactersHolder);
                selectButtonInstance.SetCharacter(this, character);
                characterButtons.Add(selectButtonInstance);
            }

            //Set the positions for the buttons
            foreach (var button in characterButtons)
            {
                Vector2 newLastPosition;
                newLastPosition = new Vector2(lastButtonPosition.x + 100, lastButtonPosition.y);
                button.transform.position = newLastPosition;
                lastButtonPosition = newLastPosition;
            }

            players.OnListChanged += HandlePlayerStateChanges;
        }

        if(IsServer)
        {
            //We add a method to the on client connect
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            //We add a method to the on client disconnect
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            //So that w do not miss any clients that spawned earlier or later in the game
            foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }

        //Unity Relay System
        if(IsHost)
        {
            joinCodeText.text = ServerManager.Instance.JoinCode;
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient)
        {
            players.OnListChanged -= HandlePlayerStateChanges;
        }

        if(IsServer)
        {
            //We remove the method from the on client connect
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            //We remove the method from the on client disconnect
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new CharacterSelectState(clientId));
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].ClientId == clientId)
            {
                players.RemoveAt(i);
                break;
            }
        }
    }

    public void Select(Character character /*CharacterClass characterClass*/)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (players[i].IsLockedIn) { return; }

            if (players[i].CharacterId == character.Id) { return; }

            if (IsCharacterTaken(character.Id, false)) { return; }
        }

        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(true);

        if (introInstance != null)
        {
            Destroy(introInstance);
        }

        introInstance = Instantiate(character.IntroPrefab, introSpawnPoint);

        SelectServerRpc(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(characterId)) { return; }
            //if(!characterDatabaseClass.IsValidCharacterId(characterId)) {return;}

            if (IsCharacterTaken(characterId, true)) { return; }

            players[i] = new CharacterSelectState(
                players[i].ClientId,
                characterId,
                players[i].IsLockedIn
            );
        }
    }

    public void LockIn()
    {
        LockInServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(players[i].CharacterId)) { return; }
            //if(!characterDatabaseClass.IsValidCharacterId(characterId)) {return;}

            if (IsCharacterTaken(players[i].CharacterId, true)) { return; }

            players[i] = new CharacterSelectState(
                players[i].ClientId,
                players[i].CharacterId,
                true
            );
        }

        foreach (var player in players)
        {
            //if one player is not locked in yet we wait
            if(!player.IsLockedIn) {return;}
        }

        foreach (var player in players)
        {
            //As soon as all characters are locked in we Set the character for every single player
            ServerManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
        }

        //Then we start the game
        ServerManager.Instance.StartGame();
    }

    private void HandlePlayerStateChanges(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if(players.Count > i)
            {
                playerCards[i].UpdateDisplay(players[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }

        foreach (var button in characterButtons)
        {
            if(button.IsDisabled) {continue;}

            if(IsCharacterTaken(button.Character.Id, false))
            {
                button.SetDisabled();
            }
        }

        foreach (var player in players)
        {
            if(player.ClientId != NetworkManager.Singleton.LocalClientId) {continue;}

            if(player.IsLockedIn)
            {
                lockInButton.interactable = false;
                break;
            }

            if(IsCharacterTaken(player.CharacterId, false))
            {
                lockInButton.interactable = false;
                break;
            }

            lockInButton.interactable = true;

            break;
        }
    }

    private bool IsCharacterTaken(int characterId, bool checkAll)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(!checkAll)
            {
                if(players[i].ClientId == NetworkManager.Singleton.LocalClientId) {continue;}

                if(players[i].IsLockedIn && players[i].CharacterId == characterId)
                {
                    return true;
                }

            }
        }

        return false;
    }
}
