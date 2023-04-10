using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerHead;

    private void Update() 
    {
        transform.position = playerHead.transform.position;
    }
}
