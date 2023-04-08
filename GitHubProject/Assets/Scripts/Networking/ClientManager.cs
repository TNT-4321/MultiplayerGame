using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance {get; private set;}

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

    /*
    //Without Unity Relay System
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    */

    //With Unity Relay System
    public async void StartClient(string joinCode)
    {
        JoinAllocation allocation;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay get join code request failed");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        var relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
    }
}
