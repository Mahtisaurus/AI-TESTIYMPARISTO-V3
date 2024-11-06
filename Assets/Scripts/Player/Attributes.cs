using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    [Header("HP")]
    public float maxHealth;
    public float currentHealth;

    [Header("STAMINA")]
    public float maxStamina;
    public float currentStamina;

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            Debug.Log("I am dead");
        }
    }
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

}
