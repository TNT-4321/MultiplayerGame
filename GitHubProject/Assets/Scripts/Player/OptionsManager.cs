using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public PlayerNetworkController player;

    [Header("MouseSensitivity")]
    [SerializeField]private TMP_Text displaySensytivity;
    [SerializeField]private Slider sensitivitySlider;
    [SerializeField] private float maxSensitivity = 1000f;

    private void Start() 
    {
        //Set references
    }

    private void Update() 
    {
        if(gameObject.activeInHierarchy)
        {
            player = FindObjectOfType<PlayerNetworkController>();

            HandleMouseSensitivityChange();
        }
    }

    private void HandleMouseSensitivityChange()
    {
        sensitivitySlider.maxValue = maxSensitivity;
        player.sensX = sensitivitySlider.value;
        player.sensY = sensitivitySlider.value;
    }
}
