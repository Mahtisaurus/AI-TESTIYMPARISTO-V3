using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform playerObject;
    private Rigidbody rb;
    private PlayerMovementAdvanced playerMoveScript;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;



    // Start is called before the first frame update
    void Start()
    {
        playerMoveScript = GetComponent<PlayerMovementAdvanced>();
        playerMoveScript.sliding = false;

        rb = GetComponent<Rigidbody>();
        startYScale = playerObject.localScale.y;
    }


    // Update is called once per frame
    void Update()
    {
        // A + D
        horizontalInput = Input.GetAxisRaw("Horizontal");
        // W + S
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();

        if (Input.GetKeyUp(slideKey) && playerMoveScript.sliding)
            StartSlide();
    }


    private void FixedUpdate()
    {
        SlidingMovement();
    }



    private void StartSlide()
    {
        playerMoveScript.sliding = true;

        playerObject.localScale = new Vector3(playerObject.localScale.x, slideYScale, playerObject.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Sliding normal
        if(!playerMoveScript.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        // Sliding on slope. No need for timer here! We can slide as long as we want on a slope :D  also sliding can be coded differently if Apex Legends type sliding is desired.
        else
        {
            rb.AddForce(playerMoveScript.GetSlopeMoveDirection(inputDir) * slideForce, ForceMode.Force);
        }


        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        playerMoveScript.sliding = false;

        playerObject.localScale = new Vector3(playerObject.localScale.x, startYScale, playerObject.localScale.z);
    }
}
