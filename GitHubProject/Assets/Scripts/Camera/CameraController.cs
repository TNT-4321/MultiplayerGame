using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerHead;

    public enum FollowState
    {
        FPS,
        ThirdPerson,
        Driving
    }

    public FollowState followState;

    private void Update() 
    {
        switch (followState)
        {
            case FollowState.FPS:
                FPSFollow();
                break;
            case FollowState.Driving:
                DrivingFollow();
                break;
        }
    }

    private void FPSFollow()
    {
        transform.position = playerHead.transform.position;
    }

    private void DrivingFollow()
    {
        transform.position = new Vector3(playerHead.transform.position.x, playerHead.transform.position.y, playerHead.transform.position.z - 5f);
    }
}
