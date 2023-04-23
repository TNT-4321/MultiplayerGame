using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Currency : MonoBehaviour
{
    [Header("IngameDisplay")]
    private TMP_Text playerCoinsDisplayText;
    private int currentCoins = 0;
    
    private void Start() 
    {
        //Set references
        playerCoinsDisplayText = GameObject.Find("MoneyDisplay").GetComponent<TMP_Text>();
    }

    public void AddCoins(int numberOfCoinsToAdd)
    {
        currentCoins += numberOfCoinsToAdd;
        playerCoinsDisplayText.text = currentCoins.ToString();
    }

    public void RemoveCoins(int numberOfCoinsToRemove)
    {
        currentCoins -= numberOfCoinsToRemove;
        playerCoinsDisplayText.text = currentCoins.ToString();
    }
}
