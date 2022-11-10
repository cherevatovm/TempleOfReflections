using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_controller : MonoBehaviour
{
    [SerializeField] float MP_impact;
    [SerializeField] HealthSystem healthSystem;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Игрок подошел");
            ChangeCurrentMP();
        }
    }

    void ChangeCurrentMP()
    {
        healthSystem.currentMP += MP_impact;
        if (healthSystem.currentMP > healthSystem.maxMP)
            healthSystem.currentMP = healthSystem.maxMP;
        if (!healthSystem.DoesHaveMP())
            healthSystem.OutOfMP();
    }
}
