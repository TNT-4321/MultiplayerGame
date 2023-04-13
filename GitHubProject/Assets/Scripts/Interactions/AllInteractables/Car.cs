using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    private bool isInteractable;

    public string interactionPrompt => prompt;
    public bool canBeInteractedWith => isInteractable;

    private void Start() 
    {
        isInteractable = true;
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Enter car");
        isInteractable = false;

        interactor.transform.position = new Vector3(0, 20f, 0);
        interactor.player.GetComponent<Rigidbody>().velocity = new Vector3(0, 100f, 0);

        return true;
    }
}
