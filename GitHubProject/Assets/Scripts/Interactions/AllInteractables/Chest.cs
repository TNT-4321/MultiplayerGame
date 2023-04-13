using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    private bool isInteractable;

    public string interactionPrompt => prompt;

    public bool canBeInteractedWith => isInteractable;

    private int coinsInChest;

    private void Start() 
    {
        isInteractable = true;
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Opening chest");

        coinsInChest = Random.Range(1, 20);
        interactor.transform.position = new Vector3(5f, 5f, 0);
        interactor.currency.AddCoins(coinsInChest);

        return true;
    }
}
