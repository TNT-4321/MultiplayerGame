using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius;
    [SerializeField] private LayerMask interactionLayer;

    private readonly Collider[] colliders = new Collider[2];
    [SerializeField] private int numCollidersFound;

    private void Update() 
    {
        numCollidersFound = Physics.OverlapSphereNonAlloc(interactionPoint.transform.position, interactionPointRadius, colliders, interactionLayer);
    }
}
