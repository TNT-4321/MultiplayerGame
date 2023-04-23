using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class PlayerStatsController : NetworkBehaviour
{
    [Header("Health")]
    private Slider healthSlider;
    [SerializeField] private int maxHealth;
    private int currentHealth;

    private void Start() 
    {
        //Set referneces
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        //Set variables
        currentHealth = maxHealth;
    }

    private void Update() 
    {
        if(!IsOwner) {return;}

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        //For testing
        if(Input.GetKeyDown(KeyCode.M))
        {
            currentHealth -= 10;
        }
    }
}
