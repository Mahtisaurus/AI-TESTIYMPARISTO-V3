using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script includes all player movement related actions:
/// - Move
/// - Jump
/// - Drag control
/// - Speed control
/// - Slope movement
/// - Sprint
/// - Crouch
/// - Slide (see Sliding.cs for more)
/// - Momentum (very big thing!)
/// - 
/// (List up to date)
/// </summary>

public class PlayerMovementAdvanced : MonoBehaviour
{
    // [Header("")]


    // NOTEWORTHY:
    // 1. Slopessa pelaaja liikkuu paljon kovempaa, koska SpeedControl lasketaan vain horizontaalisen muutoksen mukaan. Kuvittele perus suorakulmainen kolmio. Nyt lasku tapahtuu maata vasten kulkevassa sivussa, mutta slope on ns. hypotenuusa, eli kuljettu matka on suurempi mutta sama horizontaalisesti
    //      -> Korjaus 1. kohtaan on olemassa jo SpeedControllissa, mutta kommentoituna. Slopessa nopeemmin liikkuminen on ihan cool:D
    // 2. Kyykyssä juokseminen on yhtä nopeaa kuin normi juoksu
    //      -> Helppo korjata parilla if lauseella, mutta ihan hauska mekaniikka toistaseks. Slideeminen on myös jo asia niin ei tarvi tota periaatteessa olla.


    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float sneakSpeed;

    public bool sliding;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airControlMultiplier;
    bool canJump = true;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYscale;
    private float startYscale;


    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode sneakKey = KeyCode.LeftControl;


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Head Check")]
    public Transform headCheck;
    public float headCheckDistance = 0.4f;
    public bool isAbleToStand;
    public bool standWhenPossible;

    [Header("Slope Handler")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Player Related")]
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    public float playerHeight;
    public GameObject body;
    
    Vector3 moveDir;
    Rigidbody rb;

    public MovementState moveState;

    public enum MovementState
    {
        WALKING,
        SPRINTING,
        CROUCHING,
        SLIDING,
        AIR,
        SNEAKING
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYscale = transform.localScale.y;

        standWhenPossible = false;
        isAbleToStand = true;
    }

    private void Update()
    {
        // Ground check with CheckSphere gameObject
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Checking if crouching and able to stand
        if (moveState == MovementState.CROUCHING)
        {
            isAbleToStand = !Physics.CheckSphere(headCheck.position, headCheckDistance, groundLayer);
        }

        MyInput();
        SpeedControl();
        StateHandler();


        // Drag calculations (maybe its own function later on?? :D)
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // Turn the players body towards orientation and correct 90 degree error
        body.transform.rotation = orientation.rotation;
        body.transform.rotation *= Quaternion.Euler(0, -90, 0);
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void StateHandler()
    {
        // SLIDING
        if (sliding)
        {
            moveState = MovementState.SLIDING;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }
        // SPRINTING
        else if (isGrounded && Input.GetKey(sprintKey) && moveState != MovementState.CROUCHING)
        {
            moveState = MovementState.SPRINTING;
            desiredMoveSpeed = sprintSpeed;
        }
        // CROUCHING
        else if (Input.GetKey(crouchKey))
        {
            moveState = MovementState.CROUCHING;
            desiredMoveSpeed = crouchSpeed;
        }
        // CROUCHING BUT STUFF ABOVE HEAD
        else if (moveState == MovementState.CROUCHING && !isAbleToStand)
        {
            moveState = MovementState.CROUCHING;
            desiredMoveSpeed = crouchSpeed;
        }
        // SNEAKING
        else if (isGrounded && Input.GetKey(sneakKey))
        {
            moveState = MovementState.SNEAKING;
            desiredMoveSpeed = sneakSpeed;
        }
        // WALKING
        else if (isGrounded && isAbleToStand)
        {
            moveState = MovementState.WALKING;
            desiredMoveSpeed = walkSpeed;
        }
        // AIR
        else
        {
            moveState = MovementState.AIR;
        }


        // Check if desiredMoveSpeed has changed a lot
        // This means that if your speed change is below 4f, the change will be instant (for ex. WALKING -> SPRINTING)
        // Then if the speed change is over 4f, the change will be over time! GGEZ
        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }


    // Checks all the different player inputs
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Hyppää Harri!
        if (Input.GetKey(jumpKey) && canJump && isGrounded && moveState != MovementState.CROUCHING)
        {
            canJump = false;

            Jump();

            // Harri! Voit hyvätä uudestaan vasta hetken kuluttua! #Tulisiemeniä
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Jalkapäivän kyykkysessiot
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);

            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            standWhenPossible = false;
        }

        if (Input.GetKeyUp(crouchKey) && isAbleToStand)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
            standWhenPossible = false;
        }

        if (Input.GetKeyUp(crouchKey) && !isAbleToStand)
        {
            // NOT ABLE TO STAND! STUFF ABOVE HEAD
            standWhenPossible = true;
        }

        // Can stand and will stand when possible from crouching state!
        if (isAbleToStand && !Input.GetKey(crouchKey) && standWhenPossible && moveState == MovementState.CROUCHING){
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        }
    }

    // Lerps moveSpeed over time to desiredMoveSpeed value. This is used to hold momentum! Very cool!
    private IEnumerator SmoothLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            
            // Greater momentum depending on slope angle
            if(OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }


    // This function moves the player according to inputs
    private void MovePlayer()
    {
        // Calculate movement direction based on Orientation component (see CameraLook.cs for more)
        // Add force aka. Move to the direction you are looking at
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDir) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // On ground
        if (isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);

        // On air
        else if (!isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airControlMultiplier, ForceMode.Force);

        // Jottei harjotella slideemistä paikallaan slopessa
        rb.useGravity = !OnSlope();
    }


    private void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 currentVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // "But officer, I was only following the BMW"
            if (currentVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = currentVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // Reset Y velocity on jump so you always jump same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    private void ResetJump()
    {
        canJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }


    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
