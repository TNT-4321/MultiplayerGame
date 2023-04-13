using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public PlayerNetworkController player;

    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse1;

    private readonly Collider[] colliders = new Collider[2];
    [SerializeField] private int numCollidersFound;

    private void Update() 
    {
        numCollidersFound = Physics.OverlapSphereNonAlloc(interactionPoint.transform.position, interactionPointRadius, colliders, interactionLayer);

        if(numCollidersFound > 0)
        {
            var interactable = colliders[0].GetComponent<IInteractable>();

            if(!interactable.canBeInteractedWith) {return;}

            if(interactable != null && Input.GetKeyDown(interactKey))
            {
                interactable.Interact(this);
            }
        }   
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.transform.position, interactionPointRadius);
    }
}
