using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentDoor : MonoBehaviour
{
    [SerializeField] private Animator ventDoor = null;
    [SerializeField] private bool doorOpen = true;

    public void ToggleDoor()
    {
        //gameObject.GetComponent<Collider>().enabled = false;

        if (doorOpen == false)
        {
            ventDoor.Play("VentDoorOpen", 0, 0.0f);
            doorOpen = true;
        }
        else if (doorOpen == true)
        {
            ventDoor.Play("VentDoorClose", 0, 0.0f);
            doorOpen = false;
        }

        //gameObject.GetComponent<Collider>().enabled = true;
    }
}