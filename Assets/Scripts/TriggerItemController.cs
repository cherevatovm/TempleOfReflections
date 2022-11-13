using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemController : MonoBehaviour
{
    [SerializeField] bool isHealthAffectingItem;//if == true, then the item restores/damage HP, if == false, MP 
    [SerializeField] int Impact;
    [SerializeField] Unit playerUnit;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Игрок подошел");
            if (isHealthAffectingItem)
                ChangeCurrentHealth();
            else
                ChangeCurrentMP();
        }
    }

    void ChangeCurrentHealth()
    {
        playerUnit.currentHP += Impact;
        if (playerUnit.currentHP > playerUnit.maxHP)
            playerUnit.currentHP = playerUnit.maxHP;
        if (playerUnit.IsDead())
            playerUnit.Death();
    }

    void ChangeCurrentMP()
    {
        playerUnit.currentMP += Impact;
        if (playerUnit.currentMP > playerUnit.maxMP)
            playerUnit.currentMP = playerUnit.maxMP;
    }
}
