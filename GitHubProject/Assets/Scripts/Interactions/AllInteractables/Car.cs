using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour, IInteractable, ICar
{
    [SerializeField] private string prompt;
    private bool isInteractable;

    public string interactionPrompt => prompt;
    public bool canBeInteractedWith => isInteractable;

    [Header("CarSpecificReferences")]
    [SerializeField] private Transform driverSeatPosition;

    private void Start() 
    {
        isInteractable = true;
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Interacted with car");
        isInteractable = false;

        return true;
    }

    private float xInput;
    private float yInput;
    
    public float XInput => xInput;
    public float YInput => yInput;

    public bool StartDriving(PlayerNetworkController driver) 
    {
        //Here the player has to enter the car
        driver.GetComponent<CapsuleCollider>().enabled = false;
        driver.driverSeatPosition = driverSeatPosition;
        driver.playerState = PlayerNetworkController.PlayerState.Driving;
        driver.camState = PlayerNetworkController.CamState.Car;
        driver.camController.followState = CameraController.FollowState.Driving;
        Debug.Log("Enter car");
        return true;
    }

    private void Update() 
    {
        ExecuteDriving();
    }

    private void ExecuteDriving()
    {
        Debug.Log("Execute driving");
    }

    public bool StopDriving(PlayerNetworkController driver)
    {
        driver.transform.position = new Vector3(driver.transform.position.x + 5, driver.transform.position.y, driver.transform.position.z - 5);
        driver.GetComponent<CapsuleCollider>().enabled = true;
        driver.driverSeatPosition = null;
        driver.playerState = PlayerNetworkController.PlayerState.Normal;
        driver.camState = PlayerNetworkController.CamState.FPS;
        driver.camController.followState = CameraController.FollowState.FPS;
        driver = null;

        if(driver == null)
        {
            Debug.Log("driver is null");
            return true;
        }   
        else
            Debug.Log("Driver not null");
            return false;
    }
}
