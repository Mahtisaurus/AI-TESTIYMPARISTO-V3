using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetDoor2 : MonoBehaviour
{
    [SerializeField] private Animator closetDoor2 = null;
    [SerializeField] private bool doorOpen = false;

    public void ToggleDoor()
    {
        gameObject.GetComponent<Collider>().enabled = false;

        if (!doorOpen)
        {
            closetDoor2.Play("ClosetDoorOpen2", 0, 0.0f);
        }
        else if (doorOpen)
        {
            closetDoor2.Play("ClosetDoorClose2", 0, 0.0f);
        }

        gameObject.GetComponent<Collider>().enabled = true;
        doorOpen = !doorOpen;
    }
}