using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Interactor : NetworkBehaviour
{
    public PlayerNetworkController player;

    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse1;

    private readonly Collider[] colliders = new Collider[2];
    [SerializeField] private int numCollidersFound;

    private ICar currentCar = null;

    [Header("IngameCurrency")]
    public Currency currency;

    private void Update() 
    {
        if(!IsOwner) {return;}

        numCollidersFound = Physics.OverlapSphereNonAlloc(interactionPoint.transform.position, interactionPointRadius, colliders, interactionLayer);

        if(numCollidersFound > 0)
        {
            var interactable = colliders[0].GetComponent<IInteractable>();
            currentCar = colliders[0].GetComponent<ICar>();

            if(!interactable.canBeInteractedWith) {return;}

            if(interactable != null && Input.GetKeyDown(interactKey))
            {
                interactable.Interact(this);
            }
        }

        

        if(currentCar != null)
        {
            currentCar.StartDriving(player);

            if(Input.GetKeyDown(KeyCode.N))
            {
            currentCar.StopDriving(player);
            currentCar = null;
            Debug.Log("Car null");
            }
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.transform.position, interactionPointRadius);
    }
}
