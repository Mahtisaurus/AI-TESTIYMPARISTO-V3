using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetDoor : MonoBehaviour
{
    [SerializeField] private Animator closetDoor = null;
    [SerializeField] private bool doorOpen = false;

    public void ToggleDoor()
    {
        gameObject.GetComponent<Collider>().enabled = false;

        if (!doorOpen)
        {
            closetDoor.Play("ClosetDoorOpen", 0, 0.0f);
        }
        else if (doorOpen)
        {
            closetDoor.Play("ClosetDoorClose", 0, 0.0f);
        }

        gameObject.GetComponent<Collider>().enabled = true;
        doorOpen = !doorOpen;
    }
}
