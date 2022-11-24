using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public void CombatAI()
    {
        int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
        CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
        CombatSystem.instance.combatUI.combatDialogue.text = "Враг наносит " + totalDamage + " физического урона";
    }
}
