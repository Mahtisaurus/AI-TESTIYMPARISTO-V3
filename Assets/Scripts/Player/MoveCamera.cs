using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Player Camera is intentionally attached to another object and uses this script to move with the player on purpose.
/// This script combined with the above allows the camera to move a lot smoother.
/// </summary>


public class MoveCamera : MonoBehaviour
{
    public Transform cameraPos;


    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPos.position;
    }
}
