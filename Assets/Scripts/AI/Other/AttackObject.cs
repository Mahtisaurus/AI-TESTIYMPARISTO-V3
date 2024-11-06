using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    public float damage = 45;
    public float lingerTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, lingerTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player hit " + damage);
            other.gameObject.GetComponentInParent<Attributes>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}