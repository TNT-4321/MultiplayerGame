using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkController : NetworkBehaviour
{
    [Header("CameraMovement")]
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Camera playerCam;

    private float xRotation;
    private float yRotation;

    public enum CamState
    {
        FPS,
        ThirdPerson,
        Car
    }

    [Header("PlayerMovement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float groundDrag = 5f;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rigidbody;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Get components
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //Set values
        jumpTimer = jumpCooldown;
    }

    private void Update() 
    {
        if(!IsOwner)
        {
            playerCam.gameObject.SetActive(false);
            return;
        }

        FPSCameraMovement();

        MoveInput();
        SpeedControl();
        HandleDrag();

        //Reset jump
        jumpTimer -= Time.deltaTime;
        if(jumpTimer <= 0)
        {
            readyToJump = true;
        }

        //For testing
        MakeCursorVisible();
    }

    private void FixedUpdate() 
    {
       if(!IsOwner) {return;} 

       MovePlayer();
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
        //calculate move direction
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        //move the rigidbody in this direction 
        //ground movement
        if(IsGrounded())
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        //air movement
        else if(!IsGrounded())
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f *  airMultiplier, ForceMode.Force);  
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

    private void SpeedControl()
    {
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
    }

    private void ResetJump()
    {
        readyToJump = true;
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

    //Testing
    private bool cursorIsVisible = false;
    private void MakeCursorVisible()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            cursorIsVisible = !cursorIsVisible;

        if(cursorIsVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
