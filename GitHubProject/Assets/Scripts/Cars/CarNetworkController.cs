using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CarNetworkController : NetworkBehaviour
{
    private Rigidbody carRigidbody;
    public WheelColliders colliders;
    public WheelMeshes meshes;

    [Header("Car")]
    private float gasInput;
    private float steeringInput;

    [SerializeField] private float motorPower;
    private float speed;
    [SerializeField] private AnimationCurve steeringCurve;

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
    }

    private void ApplyMotor()
    {
        colliders.BLWheel.motorTorque = motorPower * gasInput;
        colliders.BRWheel.motorTorque = motorPower * gasInput;
    }

    private void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);

        colliders.FLWheel.steerAngle = steeringAngle;
        colliders.FRWheel.steerAngle = steeringAngle;
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
    }
}

[System.Serializable]
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
}
