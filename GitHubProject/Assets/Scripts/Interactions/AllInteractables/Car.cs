using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Car : NetworkBehaviour, IInteractable, ICar
{
    [SerializeField] private string prompt;
    private bool isInteractable;

    public string interactionPrompt => prompt;
    public bool canBeInteractedWith => isInteractable;

    [Header("CarSpecificReferences")]
    [SerializeField] private Transform driverSeatPosition;
    private Rigidbody carRigidbody;

    //Just handles the Input
    private PlayerNetworkController driverInput = null;

    [Header("CarController")]
    private float horizontalInput;
    private float verticalInput;
    private bool isBreaking;
    [SerializeField] private KeyCode breakingKey = KeyCode.Space;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;

    [SerializeField] private float motorPower;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float breakForce;
    private float currentBreakForce;
    private float currentSteerAngle;

    private void Start() 
    {
        isInteractable = true;

        carRigidbody = GetComponent<Rigidbody>();
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Interacted with car");
        isInteractable = false;

        return true;
    }

    public bool StartDriving(PlayerNetworkController driver) 
    {
        //Here the player has to enter the car
        
        driverInput = driver;
        //Disable the collider of the player
        driver.GetComponent<CapsuleCollider>().enabled = false;
        //transform his position to the seat
        driver.driverSeatPosition = driverSeatPosition;
        //Change states properly
        driver.playerState = PlayerNetworkController.PlayerState.Driving;
        driver.camState = PlayerNetworkController.CamState.Car;
        driver.camController.followState = CameraController.FollowState.Driving;
        Debug.Log("Enter car");
        return true;
    }

    private void Update() 
    {
        DriveInput();
        Debug.Log(horizontalInput + ":" + verticalInput);
    }

    private void FixedUpdate() 
    {
        HandleMotorServerRpc();
        HandleSteeringServerRpc();
    }

    private void DriveInput() 
    {
        if(driverInput != null)
        {
            horizontalInput = driverInput.horizontalInput;
            verticalInput = driverInput.verticalInput;
            isBreaking = Input.GetKey(breakingKey);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleMotorServerRpc()
    {
        HandleMotorClientRpc();
    }

    [ClientRpc]
    private void HandleMotorClientRpc()
    {
        if(driverInput != null)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorPower;
            frontRightWheelCollider.motorTorque = verticalInput * motorPower;
            currentBreakForce = isBreaking ? breakForce : 0f;

            if(isBreaking)
            {
                ApplyBreaking();
            }
        }
    }

    private void ApplyBreaking()
    {
        Debug.Log("IsBeraking");
        frontLeftWheelCollider.brakeTorque = currentBreakForce;
        frontRightWheelCollider.brakeTorque = currentBreakForce;
        backLeftWheelCollider.brakeTorque = currentBreakForce;
        backRightWheelCollider.brakeTorque = currentBreakForce;
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleSteeringServerRpc()
    {
        HandleSteeringClientRpc();
    }

    [ClientRpc]
    private void HandleSteeringClientRpc()
    {
        if(driverInput != null)
        {
            currentSteerAngle = maxSteeringAngle * horizontalInput;

            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;
        }
    }

    public bool StopDriving(PlayerNetworkController driver)
    {
        //Transform the player position next to the car
        driver.transform.position = new Vector3(driver.transform.position.x + 5, driver.transform.position.y, driver.transform.position.z - 5);
        //enable the collider
        driver.GetComponent<CapsuleCollider>().enabled = true;
        //Change the seat position of the player to null so that another player can enter the car
        driver.driverSeatPosition = null;
        //Change states
        driver.playerState = PlayerNetworkController.PlayerState.Normal;
        driver.camState = PlayerNetworkController.CamState.FPS;
        driver.camController.followState = CameraController.FollowState.FPS;
        //set the driver to null
        driver = null;
        driverInput = null;

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
