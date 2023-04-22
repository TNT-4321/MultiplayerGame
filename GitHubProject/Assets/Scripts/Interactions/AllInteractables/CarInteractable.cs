using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CarInteractable : Interactable
{
    public override bool canBeInteractedWith { get; set; }
    private Interactor currentInteractor;

    private void Start() 
    {
        canBeInteractedWith = true;
        //To make sure the client id is not 0 he is is automatically owner
        ChangeOwnershipServerRpc(10000);
    }

    public override void OnInteraction(Interactor interactor)
    {
        currentInteractor = interactor;
        EnterCar();
    }

    private void EnterCar()
    {
        canBeInteractedWith = false;
        Debug.Log("Enter car");
    }

    private void Update() 
    {
        if(!IsOwner) {return;}


        if(Input.GetKeyDown(KeyCode.N))
        {
            ExitCar();
        }
    }

    private void ExitCar()
    {
        currentInteractor.player.playerState = PlayerNetworkController.PlayerState.Normal;
        currentInteractor = null;
        canBeInteractedWith = true;
        //Set the ownerId to 0 so the player is not the owner anymore
        ChangeOwnershipServerRpc(10000);
        Debug.Log("Exit Car");
    }

    //Real code
    [ServerRpc(RequireOwnership = false)]
    public void ChangeOwnershipServerRpc(ulong newOwnerId)
    {
        GetComponent<NetworkObject>().ChangeOwnership(newOwnerId);
        //isInteractable = false;
        Debug.Log("Ownership changed" + newOwnerId);
    }
}
