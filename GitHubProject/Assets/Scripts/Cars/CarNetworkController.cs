using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CarNetworkController : NetworkBehaviour
{
    /*private Rigidbody carRigidbody;
    public WheelColliders colliders;
    public WheelMeshes meshes;

    [Header("Input")]
    private float gasInput;
    private float steeringInput;
    [SerializeField] private KeyCode brakeKey = KeyCode.Space;

    [Header("MotorStrenght")]
    [SerializeField] private float motorPower;
    private float speed;
    [SerializeField] private float maxSteeringAngle;

    [Header("Breaking")]
    private bool isBreaking;
    private float currentBrakeForce;
    [SerializeField] private float brakeForce;

    // Start is called before the first frame update
    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) {return;}

        speed = carRigidbody.velocity.magnitude;

        DriveInput();
        ApplyMotor();
        ApplySteering();
        UpdateWheelVisuals();
    }

    private void DriveInput()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        isBreaking = Input.GetKeyDown(brakeKey);
    }

    private void ApplyMotor()
    {
        colliders.BLWheel.motorTorque = motorPower * gasInput;
        colliders.BRWheel.motorTorque = motorPower * gasInput;

        currentBrakeForce = isBreaking ? brakeForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        colliders.FLWheel.brakeTorque = currentBrakeForce;
        colliders.FRWheel.brakeTorque = currentBrakeForce;
        colliders.BLWheel.brakeTorque = currentBrakeForce;
        colliders.BRWheel.brakeTorque = currentBrakeForce;
    }

    private void ApplySteering()
    {
        float currentSteering = steeringInput * maxSteeringAngle;

        colliders.FLWheel.steerAngle = currentSteering;
        colliders.FRWheel.steerAngle = currentSteering;
    }

    private void UpdateWheelVisuals()
    {
        UpdateWheel(colliders.FLWheel, meshes.FLWheel);
        UpdateWheel(colliders.FRWheel, meshes.FRWheel);
        UpdateWheel(colliders.BLWheel, meshes.BLWheel);
        UpdateWheel(colliders.BRWheel, meshes.BRWheel);
    }

    private void UpdateWheel(WheelCollider collider, MeshRenderer mesh)
    {
        Quaternion quaternion;
        Vector3 position;
        collider.GetWorldPose(out position, out quaternion);
        mesh.transform.rotation = quaternion;
    }*/
}

/*[System.Serializable]
public class WheelColliders
{
    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider BLWheel;
    public WheelCollider BRWheel;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FLWheel;
    public MeshRenderer FRWheel;
    public MeshRenderer BLWheel;
    public MeshRenderer BRWheel;
}*/
