using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Interactor : NetworkBehaviour
{
    public PlayerNetworkController player;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    private Interactable currentFocusedInteractable;
    public CarInteractable myCar;
    public Currency myCurrency;
    public CameraController followCam;
    [SerializeField] private Vector3 rayPoint;
    [SerializeField] private float interactionRange;
    [SerializeField] private Camera playerCam;
    [SerializeField] private LayerMask interactablesLayer;

    private void Start() 
    {
        myCurrency = GetComponent<Currency>();
    }
    
    private void Update() 
    {
        if(!IsOwner) {return;}

        InteractionCheck();
        InteractionInput();
    }

    private void InteractionCheck()
    {
        if(Physics.Raycast(playerCam.ViewportPointToRay(rayPoint),out RaycastHit hit, interactionRange, interactablesLayer))
        {
            //We found an interactable and set it to our currentInteractable
            hit.collider.TryGetComponent(out currentFocusedInteractable);
            Debug.Log("this is" + currentFocusedInteractable);
        }
        else if (currentFocusedInteractable)
        {
            //We did not find an interactable
            currentFocusedInteractable = null;
        }
    }

    private void InteractionInput()
    {
        if(Input.GetKeyDown(interactionKey) && currentFocusedInteractable != null && currentFocusedInteractable.canBeInteractedWith)
        {
            Debug.Log("Hello");
            currentFocusedInteractable.OnInteraction(this);

            currentFocusedInteractable.TryGetComponent(out myCar);

            if(myCar != null)
            {
                myCar.ChangeOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
                followCam.followState = CameraController.FollowState.Driving;
                player.playerState = PlayerNetworkController.PlayerState.Driving;
                player.DeactivateVisualsAndCollider();
                player.driverSeatPosition = myCar.driverSeat;
            }      
        }
    }
}
