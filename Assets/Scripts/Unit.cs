using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public float attackStrength;
    public float currentHP;
    public float maxHP;

    public void TakeDamage(float damage) => currentHP -= damage;

    public bool IsDead() => currentHP <= 0;
}
