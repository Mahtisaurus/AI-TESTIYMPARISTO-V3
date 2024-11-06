using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public string interactableName = "undefined";

    public InteractableType interactableType;

    public enum InteractableType
    {
        TEST,
        CLOSET,
        CLOSET2,
        VENT,
        DOOR,
        ITEM
    }
    

    public void Interact()
    {
        switch(interactableType)
        {
            case InteractableType.TEST:
                Debug.Log("TEST");
                break;
            case InteractableType.CLOSET:
                gameObject.GetComponent<ClosetDoor>().ToggleDoor();
                break;
            case InteractableType.CLOSET2:
                gameObject.GetComponent<ClosetDoor2>().ToggleDoor();
                break;
            case InteractableType.VENT:
                gameObject.GetComponent<VentDoor>().ToggleDoor();
                break;
            case InteractableType.DOOR:
                Debug.Log("DOOR");
                break;
            case InteractableType.ITEM:
                Debug.Log("ITEM");
                break;
        }
    }

    public string GetName()
    {
        return interactableName;
    }
}
