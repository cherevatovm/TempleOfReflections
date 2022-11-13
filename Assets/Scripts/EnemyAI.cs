using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    CombatSystem combatSystem;

    void Start()
    {
        combatSystem = GameObject.Find("Combat System").GetComponent<CombatSystem>();
    }

    public void CombatAI()
    {
        int totalDamage = combatSystem.CalcAffinityDamage(0, combatSystem.enemyUnit, combatSystem.playerUnit);
        combatSystem.playerUnit.TakeDamage(totalDamage);
        combatSystem.combatDialogue.text = "Враг наносит " + totalDamage + " урона";
    }
}
