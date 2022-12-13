using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemController : MonoBehaviour
{
    [SerializeField] bool isHealthAffectingItem;
    [SerializeField] int Impact;
    [SerializeField] Unit playerUnit;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isHealthAffectingItem)
                ChangeCurrentHealth();
            else
                ChangeCurrentMP();
        }
    }

    void ChangeCurrentHealth()
    {
        if (Impact > 0)
            playerUnit.Heal(Impact);
        else
            playerUnit.TakeDamage(Impact * -1);
        if (playerUnit.IsDead())
            playerUnit.Death();
    }

    void ChangeCurrentMP()
    {
        if (Impact > 0)
            playerUnit.IncreaseCurrentMP(Impact);
        else
            playerUnit.ReduceCurrentMP(Impact * -1);
    }
}
