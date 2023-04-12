using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkController : NetworkBehaviour
{
    [Header("CameraMovement")]
    [SerializeField] private Vector3 camCenter;
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Camera playerCam;

    private float xRotation;
    private float yRotation;

    public enum CamState
    {
        FPS,
        ThirdPerson,
        Car,
        Freezed
    }

    public CamState camState;


    [Header("PlayerMovement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float groundDrag = 5f;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rigidbody;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    public enum PlayerState
    {
        Normal,
        Freezed,
        Grappling
    }

    private bool freezedForMoment;

    public PlayerState playerState;


    [Header("Grappling")]
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask whatIsGrappable;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;

    private Vector3 grapplePoint;

    [SerializeField] private float grapplingCd;
    private float grapplingCdTimer;

    [SerializeField] private KeyCode grappleKey = KeyCode.Mouse1;
    
    private bool isGrappling;

    public float overshootYAxis;


    [Header("GroundCheck")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;


    [Header("Jump")]
    [SerializeField] private float jumpForce = 25f;
    [SerializeField] private float jumpCooldown = 0.25f;
    private float jumpTimer;
    [SerializeField] private float airMultiplier = 0.4f;
    private bool readyToJump;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;


    [Header("Animation")]
    [SerializeField] private Animator anim;


    private void Start() 
    {
        //Make the cursor locked in the middle of the screen and invisible
        HideCursor();
        //Get components
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //Set values
        jumpTimer = jumpCooldown;
        camState = CamState.FPS;
        playerState = PlayerState.Normal;
    }

    private void Update() 
    {
        if(!IsOwner)
        {
            playerCam.gameObject.SetActive(false);
            return;
        }

        switch (camState)
        {
            case CamState.FPS:
                HideCursor();
                FPSCameraMovement();
                break;
            case CamState.ThirdPerson:
                ThirdPersonCameraMovement();
                break;
            case CamState.Freezed:
                ShowCursor();
                return;
        }

        switch (playerState)
        {
            case PlayerState.Normal:
                MoveInput();
                SpeedControl();
                HandleDrag();
                
                StartGrapple();
                break;
            case PlayerState.Freezed:
                rigidbody.velocity = Vector3.zero;
                return;
        }

        //Reset jump
        jumpTimer -= Time.deltaTime;
        if(jumpTimer <= 0)
        {
            readyToJump = true;
        }

        //Grappling timer
        if(grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;

        //For testing
        MakeCursorVisible();
    }

    private void FixedUpdate() 
    {
       if(!IsOwner) {return;} 

       MovePlayer();
    }

    private void LateUpdate() 
    {
        if(isGrappling)
            lineRenderer.SetPosition(0, gunTip.transform.position);
    }

    private void StateMashine()
    {
        //Handles changing states 
    }

    private void MoveInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Jump
        if(Input.GetKeyDown(jumpKey) && readyToJump && IsGrounded())
        {
            Jump();
        }
    }

    private void MovePlayer()
    {
        if(isGrappling) {return;}

        moveSpeed = (ShouldSprint() ? sprintSpeed : walkSpeed);

        //calculate move direction
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        //move the rigidbody in this direction 
        //ground movement
        if(IsGrounded())
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        //air movement
        else if(!IsGrounded())
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f *  airMultiplier, ForceMode.Force);

        if(freezedForMoment)
        {
            rigidbody.velocity = Vector3.zero;
        }
        
        //Animations
        /*anim.SetBool("IsRunning", ShouldSprint());  
        if(moveSpeed == walkSpeed && !ShouldSprint())
        {
            anim.SetTrigger("IsWalking");
        }
        if(moveSpeed <= 0.1f && IsGrounded())
        {
            anim.SetTrigger("Idle");
        }*/
    }

    private void HandleDrag()
    {
        //Handle drag
        if(IsGrounded())
        {
            rigidbody.drag = groundDrag;
        }
        else
        {
            rigidbody.drag = 0;
        }
    }
    
    private bool IsGrounded()
    {
        if(Physics.CheckSphere(groundCheck.transform.position, 0.2f, whatIsGround))
            return true;
        else
            return true;
            
    }

    private bool ShouldSprint()
    {
        if(Input.GetKey(sprintKey))
            return true;
        else 
            return false;
    }

    private void SpeedControl()
    {
        if(isGrappling) { return;}

        Vector3 flatVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        //limit velocity
        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rigidbody.velocity = new Vector3(limitedVelocity.x, rigidbody.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        //reset y velocity
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        //Add force
        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        jumpTimer = jumpCooldown;

        readyToJump = false; 

        //anim.SetTrigger("IsJumping");
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    //---Grappling---//
    private void StartGrapple()
    {
        if(!Input.GetKeyDown(grappleKey)) {return;}

        if(grapplingCdTimer > 0) {return;}

        playerState = PlayerState.Grappling;

        isGrappling = true;

        freezedForMoment = true;

        RaycastHit hit;

        if(Physics.Raycast(playerCam.ViewportPointToRay(camCenter), out hit, maxGrappleDistance, whatIsGrappable))
        {
            grapplePoint = hit.point;

            ExecuteGrapple();
        }
        else
        {
            grapplePoint = playerCam.transform.position + playerCam.transform.forward * maxGrappleDistance;

            StopGrapple();
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        Debug.Log("Execute grapple");
        freezedForMoment = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if(grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    private void StopGrapple()
    {
        freezedForMoment = false;

        isGrappling = false;

        grapplingCdTimer = grapplingCd;

        lineRenderer.enabled = false;

        playerState = PlayerState.Normal;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x -  startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

        Invoke(nameof(SetVelocity), 0.1f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rigidbody.velocity = velocityToSet;
        Debug.Log("Set");
    }


    private void FPSCameraMovement()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rotate cam
        playerCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //rotate player
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void ThirdPersonCameraMovement()
    {

    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Testing
    private bool cursorIsVisible = false;
    private void MakeCursorVisible()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            cursorIsVisible = !cursorIsVisible;

        if(cursorIsVisible)
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }
}
