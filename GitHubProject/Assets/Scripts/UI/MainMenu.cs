using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
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

    public void StartClient()
    {
        ClientManager.Instance.StartClient(joinCodeInputField.text);
    }
}
