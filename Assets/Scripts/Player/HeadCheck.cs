using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCheck : MonoBehaviour
{
    public bool objectAboveHead;
    public LayerMask playerLayer;

    // Start is called before the first frame update
    void Start()
    {
        objectAboveHead = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.excludeLayers.value == playerLayer)
        {
            objectAboveHead = false;
        }
        else
        {
            objectAboveHead = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.excludeLayers.value == playerLayer)
        {
            objectAboveHead = true;
        }
        else
        {
            objectAboveHead = false;
        }
    }
}
