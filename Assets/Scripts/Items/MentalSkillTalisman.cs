using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalSkillTalisman : PickableItem
{
    public int damageTypeID;

    private void Start()
    {
        isUsableInCombatOnly = true;
        isAffectingEnemy = true;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        int totalDamage = CombatSystem.instance.CalcAffinityDamage(damageTypeID, true, CombatSystem.instance.playerUnit, currentEnemyUnit);
        currentEnemyUnit.TakeDamage(totalDamage);
        currentEnemyUnit.ApplyEffect(damageTypeID - 1);
        message = damageTypeID switch
        {
            1 => CombatSystem.instance.playerUnit.unitName + " наносит " + totalDamage + " псионического урона",
            2 => CombatSystem.instance.playerUnit.unitName + " наносит " + totalDamage + " электрического урона",
            3 => CombatSystem.instance.playerUnit.unitName + " наносит " + totalDamage + " огненного урона",
            _ => string.Empty,
        };
    }
}
