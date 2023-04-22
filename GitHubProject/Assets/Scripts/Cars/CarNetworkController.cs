using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CarNetworkController : NetworkBehaviour
{
    public WheelColliders colliders;
    public WheelMeshes meshes;

    [Header("Car")]
    [SerializeField]private float gasInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) {return;}

        UpdateWheelVisuals();
    }

    private void DriveInput()
    {

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
