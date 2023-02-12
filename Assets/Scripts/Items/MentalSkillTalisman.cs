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
        CombatSystem.instance.enemyUnit.ApplyEffect(damageTypeID - 1);
        message = damageTypeID switch
        {
            1 => CombatSystem.instance.playerUnit.unitName + " ������� " + totalDamage + " ������������� �����",
            2 => CombatSystem.instance.playerUnit.unitName + " ������� " + totalDamage + " �������������� �����",
            3 => CombatSystem.instance.playerUnit.unitName + " ������� " + totalDamage + " ��������� �����",
            _ => string.Empty,
        };
    }
}
