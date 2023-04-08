using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ServerManager : NetworkBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string characterSelectSceneName = "SelectScene";
    [SerializeField] private string gameplaySceneName = "GameScene";

    public static ServerManager Instance {get; private set;}

    public Dictionary<ulong, ClientData> ClientData {get; private set;}

    //So we can make that if the game has started players can no longer join, we could also do that if someone from your team disconnects we open the game again and someone can join your team so that the game remains fair
    private bool gameHasStarted;
    [SerializeField] private int maxPlayersToJoinAGame = 4;

    private void Awake() 
    {
        //Make sure that this intance never exists twice
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //We set the Instance to this script and now we can access it from everywhere
            Instance = this;
            //That the object that this script is attached to does not get destroyed if we load a new scene 
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartServer()
    {
        //Everytime if someone joins the server this method will be called
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        //Everytime if someone joins the server this method will be called
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();
        
        NetworkManager.Singleton.StartHost();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(ClientData.Count >= maxPlayersToJoinAGame || gameHasStarted)
        {
            //No more players can join the game
            response.Approved = false;
            return;
        }

        response.Approved = true;
        //We does not want the server to instantly spawn a player prefab, because the players choose their own character
        response.CreatePlayerObject = false;
        //We are done
        response.Pending = false;

        //We add the client to our dictionary
        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);

        Debug.Log($"Added client {request.ClientNetworkId}");
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectSceneName, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(ClientData.ContainsKey(clientId))
        {
            if(ClientData.Remove(clientId))
            {
                Debug.Log($"Removed client {clientId}");
            }
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if(ClientData.TryGetValue(clientId, out ClientData data))
        {
            data.characterId = characterId;
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
