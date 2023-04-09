using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyItem lobbyItemPrefab;
    private bool isRefreshing;
    private bool isJoining;

    private void OnEnable() 
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if(isRefreshing) {return;}

        isRefreshing = true;

        try
        {
            var options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"   
                ),

                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"   
                )

            };

            var lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            //List the lobbies in the UI
            //Delete the old ones
            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }
            //Spawn in the new ones
            foreach (Lobby lobby in lobbies.Results)
            {
                var lobbyInstance = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyInstance.Initalize(this, lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isRefreshing = false;
            throw;
        }

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if(isJoining) {return;}

        isJoining =true;

        try
        {
            var joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientManager.Instance.StartClient(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isJoining = false;
            throw;
        }

        isJoining = false;
    }
}
