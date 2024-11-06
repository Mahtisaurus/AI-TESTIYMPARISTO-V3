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
/// - 
/// (List up to date)
/// </summary>

public class PlayerMovement : MonoBehaviour
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


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundLayer;
    public bool isGrounded;


    [Header("Slope Handler")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Player Related")]
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    public float playerHeight;

    Vector3 moveDir;
    Rigidbody rb;

    public MovementState moveState;

    public enum MovementState
    {
        WALKING,
        SPRINTING,
        CROUCHING,
        AIR
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYscale = transform.localScale.y;
    }

    private void Update()
    {
        // Ground check with CheckSphere gameObject
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        MyInput();
        SpeedControl();
        StateHandler();


        // Drag calculations (maybe its own function later on?? :D)
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

    }


    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void StateHandler()
    {
        // SPRINTING
        if(isGrounded && Input.GetKey(sprintKey))
        {
            moveState = MovementState.SPRINTING;
            moveSpeed = sprintSpeed;
        }
        // CROUCHING
        else if(isGrounded && Input.GetKey(crouchKey))
        {
            moveState = MovementState.CROUCHING;
            moveSpeed = crouchSpeed;
        }
        // WALKING
        else if(isGrounded)
        {
            moveState = MovementState.WALKING;
            moveSpeed = walkSpeed;
        }
        // AIR
        else
        {
            moveState = MovementState.AIR;
        }
    }


    // Checks all the different player inputs
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Hyppää Harri!
        if(Input.GetKey(jumpKey) && canJump && isGrounded)
        {
            canJump = false;
            
            Jump();

            // Harri! Voit hyvätä uudestaan vasta hetken kuluttua! #Tulisiemeniä
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Jalkapäivän kyykkysessiot
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);

            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        }

    }


    // This function moves the player according to inputs
    private void MovePlayer()
    {
        // Calculate movement direction based on Orientation component (see CameraLook.cs for more)
        // Add force aka. Move to the direction you are looking at
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDir) * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // On ground
        if(isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);

        // On air
        else if(!isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airControlMultiplier, ForceMode.Force);

        // Jottei harjotella slideemistä paikallaan slopessa
        rb.useGravity = !OnSlope();
    }


    private void SpeedControl()
    {

        /* 
        if(OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // HUOM jos tätä haluaa käyttää niin täytyy laittaa alla oleva koodi else:n sisään!

         */


        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // "But officer, I was only following the BMW"
        if(currentVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = currentVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
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
