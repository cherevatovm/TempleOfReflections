using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] float healthImpact;
    [SerializeField] HealthSystem healthSystem;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Игрок подошел");
            ChangeCurrentHealth();
        }
    }

    void ChangeCurrentHealth()
    {
        healthSystem.currentHealth += healthImpact;
        if (healthSystem.currentHealth > healthSystem.maxHealth)
            healthSystem.currentHealth = healthSystem.maxHealth;
        if (!healthSystem.IsAlive())
            healthSystem.Death();
    }
}
