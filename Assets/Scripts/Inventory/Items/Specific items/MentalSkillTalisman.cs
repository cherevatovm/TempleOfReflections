using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalSkillTalisman : PickableItem
{
    public int damageTypeID;

    private void Awake()
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
        Unit attackingUnit = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        int totalDamage = CombatSystem.instance.CalcAffinityDamage(damageTypeID, true, attackingUnit, currentEnemyUnit);
        currentEnemyUnit.TakeDamage(totalDamage);
        currentEnemyUnit.ApplyEffect(damageTypeID - 1);
        message = damageTypeID switch
        {
            1 => attackingUnit.unitName + " наносит " + totalDamage + " псионического урона",
            2 => attackingUnit.unitName + " наносит " + totalDamage + " электрического урона",
            3 => attackingUnit.unitName + " наносит " + totalDamage + " огненного урона",
            _ => string.Empty,
        };
    }
}
