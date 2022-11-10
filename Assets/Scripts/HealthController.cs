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
        healthSystem.currentHP += healthImpact;
        if (healthSystem.currentHP > healthSystem.maxHP)
            healthSystem.currentHP = healthSystem.maxHP;
        if (!healthSystem.IsAlive())
            healthSystem.Death();
    }
}
