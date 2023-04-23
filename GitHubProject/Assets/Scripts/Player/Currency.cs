using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Currency : MonoBehaviour
{
    [Header("IngameDisplay")]
    [SerializeField] private TMP_Text playerCoinsDisplayText;
    private int currentCoins = 0;
    
    public void AddCoins(int numberOfCoinsToAdd)
    {
        //Set references
        playerCoinsDisplayText = GameObject.Find("MoneyDisplay").GetComponent<TMP_Text>();

        currentCoins += numberOfCoinsToAdd;
        playerCoinsDisplayText.text = currentCoins.ToString();
    }

    public void RemoveCoins(int numberOfCoinsToRemove)
    {
        currentCoins -= numberOfCoinsToRemove;
        playerCoinsDisplayText.text = currentCoins.ToString();
    }
}
