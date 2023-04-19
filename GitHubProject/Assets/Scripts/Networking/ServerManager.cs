using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Analytics;
using Unity.Services.Core;

public class ServerManager : NetworkBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string characterSelectSceneName = "SelectScene";
    [SerializeField] private string gameplaySceneName = "GameScene";

    [SerializeField] private int maxConnections = 4;

    public static ServerManager Instance {get; private set;}

    public Dictionary<ulong, ClientData> ClientData {get; private set;}

    //Unity Relay System
    public string JoinCode {get; private set;}

    //So we can make that if the game has started players can no longer join, we could also do that if someone from your team disconnects we open the game again and someone can join your team so that the game remains fair
    private bool gameHasStarted;
    private string lobbyId;
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

    public async void StartHost()
    {
        //Unity Realy System
        Allocation allocation;

        //Unity Relay System
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch(Exception e)
        {
            Debug.Log($"Relay create allocation request failed {e.Message}");
            throw;
        }

        //Just for better Debugging
        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        //Unity Relay System
        try
        {
            //Join code 
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay get join code request failed");
            throw;
        }

        //Unity Relay System
        var relayServerData = new RelayServerData(allocation, "dtls");

        //Unity Relay System | if we publish the game on steam we cannot use the Relay system 
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        //Unity Lobby System
        try
        {
            var createLobbyOptions = new CreateLobbyOptions();
            //Later make this changable by a button
            createLobbyOptions.IsPrivate = false;
            createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                "JoinCode", new DataObject(
                    //For which people this is visible in our case just for the members of the Lobby
                    visibility: DataObject.VisibilityOptions.Member,
                    value: JoinCode
                )
                }
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby", maxConnections, createLobbyOptions);
            lobbyId = lobby.Id;
            StartCoroutine(LobbyHeartbeatCoroutine(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            throw;
        }

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

    private IEnumerator LobbyHeartbeatCoroutine(float secondsToWaitForNetxtBeat)
    {
        var delay = new WaitForSeconds(secondsToWaitForNetxtBeat);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
