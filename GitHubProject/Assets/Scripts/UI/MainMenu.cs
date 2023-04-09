//System related
using System;
using System.Collections;
using System.Collections.Generic;
//Always
using UnityEngine;
//Networking
using Unity.Netcode;
//UI
using UnityEngine.UI;
using TMPro;
//Unity Transport | Does not work if we change the transport
using Unity.Netcode.Transports.UTP;
//Unity Transport + RELAY
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
//using Lobby System
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
//Authentication
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject menuPanel;
    //Unity Relay System
    [SerializeField] private TMP_InputField joinCodeInputField;

    private async void Start() 
    {
        //Unity Relay System
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void StartHost()
    {
        ServerManager.Instance.StartHost();
    }

    public void StartServer()
    {
        ServerManager.Instance.StartServer();
    }

    public async void StartClient()
    {
        await ClientManager.Instance.StartClient(joinCodeInputField.text);
    }
}
