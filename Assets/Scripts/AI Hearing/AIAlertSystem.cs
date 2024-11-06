using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAlertSystem : MonoBehaviour, IHear
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponentInParent<PlayerMovementAdvanced>())
        {
            RespondToTouch();
        }
    }

    public void RespondToTouch()
    {
        gameObject.GetComponentInParent<AI_BEHAVE>().CanChase(true);
    }

    public void RespondToSound(Sound sound)
    {
        Debug.Log($"Heard: {sound} at {sound.pos}");
        AIManager.Instance.lastSeenPlayerLocation.position = AIManager.Instance.player.transform.position;
        gameObject.GetComponentInParent<AI_BEHAVE>().CanSearchPlayer(true);
    }
}
