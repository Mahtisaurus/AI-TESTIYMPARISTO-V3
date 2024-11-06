using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public string GetName();
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public float interactRange = 5f;
    public Crosshair crosshair;

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(interactorSource.position, interactorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObject))
            {
                crosshair.HoverText(interactObject.GetName());

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactObject.Interact();
                }
            }
            // Reset text IF it hits stuff that isnt interactable
            else
            {
                crosshair.ResetText();
            }
        }
        // Reset text IF it just doesn't hit
        else
        {
            crosshair.ResetText();
        }
    }
}