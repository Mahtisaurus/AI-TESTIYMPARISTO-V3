using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerClosetDoorController : MonoBehaviour
{
    [SerializeField] private Animator closetDoor = null;

    [SerializeField] private bool openTrigger = false;
    [SerializeField] private bool closeTrigger = false;

    // HAS TO BE MADE INTERACTABLE
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(openTrigger)
            {
                closetDoor.Play("ClosetDoorOpen", 0, 0.0f);
            }
            else if (closeTrigger)
            {
                closetDoor.Play("ClosetDoorClose", 0, 0.0f);
            }
        }
    }
}
