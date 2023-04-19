using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Car : NetworkBehaviour
{
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
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            isBreaking = Input.GetKey(breakingKey);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleMotorServerRpc()
    {
        HandleMotorClientRpc();
    }

    [ClientRpc]
    private void HandleMotorClientRpc()
    {
            frontLeftWheelCollider.motorTorque = verticalInput * motorPower;
            frontRightWheelCollider.motorTorque = verticalInput * motorPower;
            currentBreakForce = isBreaking ? breakForce : 0f;

            if(isBreaking)
            {
                ApplyBreaking();
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
    
            currentSteerAngle = maxSteeringAngle * horizontalInput;

            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    //Real code
    public void ChangeOwnership(ulong newOwnerId)
    {
        GetComponent<NetworkObject>().ChangeOwnership(newOwnerId);
        //isInteractable = false;
        Debug.Log("Ownership changed" + newOwnerId);
    }
}
