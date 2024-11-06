using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script locks and hides the cursor and takes the players mouse input and set sensitivity to turn the camera around turns the orientation of the player accordingly.
/// </summary>


public class CameraLook : MonoBehaviour
{
    public float xSens;
    public float ySens;

    public Transform playerOrientation;

    float xRotation;
    float yRotation;


    // Setting variables for sensitivity. This is just a test, can be deleted and set in inspector instead
    private void Awake()
    {
        xSens = 400;
        ySens = 400;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;


        yRotation += mouseX;
        xRotation -= mouseY;

        // Locking mouse look into up and down 90 degrees
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
