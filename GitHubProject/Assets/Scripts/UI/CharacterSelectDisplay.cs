using TMPro;
using UnityEngine;
using Unity.Netcode;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Transform charactersHolder;
    [SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private PlayerCard[] playerCards;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TextMeshProUGUI characterNameText;
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

            foreach (var character in allCharacters)
            {
                var selectButtonInstance = Instantiate(selectButtonPrefab, charactersHolder);
                selectButtonInstance.SetCharacter(this, character);
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

    public void Select(Character character)
    {
        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(true);

        SelectServerRpc(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState
                (
                    players[i].ClientId,
                    characterId
                );
            }
        }
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
    }
}
