using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public override bool canBeInteractedWith { get; set; }
    private int coinsInChest;

    private void Start() 
    {
        canBeInteractedWith = true;

        coinsInChest = Random.Range(1, 10);
    }

    public override void OnInteraction(Interactor interactor)
    {
        interactor.myCurrency.AddCoins(coinsInChest);
    }
}
