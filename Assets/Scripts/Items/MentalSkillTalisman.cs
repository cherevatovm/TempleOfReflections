using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalSkillTalisman : PickableItem
{
    public int damageTypeID;

    private void Start() => isUsableInCombatOnly = true;

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        int totalDamage = CombatSystem.instance.CalcAffinityDamage(damageTypeID, true, CombatSystem.instance.playerUnit, CombatSystem.instance.enemyUnit);
        CombatSystem.instance.enemyUnit.TakeDamage(totalDamage);
        switch (damageTypeID)
        {
            case 1:
                CombatSystem.instance.enemyUnit.PsionaEffect();
                message = CombatSystem.instance.playerUnit.unitName + " наносит " + totalDamage + " псионического урона";
                break;
            case 2:
                CombatSystem.instance.enemyUnit.ElectraEffect();
                message = CombatSystem.instance.playerUnit.unitName + " наносит " + totalDamage + " электрического урона";
                break;
            case 3:
                CombatSystem.instance.enemyUnit.FiraEffect();
                message = CombatSystem.instance.playerUnit.unitName + " наносит " + totalDamage + " огненного урона";
                break;
        }
    }
}
