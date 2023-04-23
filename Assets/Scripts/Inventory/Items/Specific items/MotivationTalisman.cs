using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotivationTalisman : ItemWithEffect
{
    private void Awake()
    {
        underEffectTurnsNumber = 3;
        isUsableInCombatOnly = true;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.tempAllyID];
        if (target.affectingItems.Exists(item => item is MotivationTalisman))
            return;
        target.affectingItems.Add(this);
        if (target.Equals(CombatSystem.instance.allyUnits[0]))
        {
            target.meleeAttackStrength += (int)(0.25 * (target as Player).initMeleeAttackStrength);
            target.mentalAttackStrength += (int)(0.25 * (target as Player).initMentalAttackStrength);
        }
        else
        {
            target.meleeAttackStrength += (int)(0.25 * target.meleeAttackStrength);
            target.mentalAttackStrength += (int)(0.25 * target.mentalAttackStrength);
        }
        message = target.unitName + " использует талисман мотивации";
    }

    public override void RemoveEffect()
    {
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        target.affectingItems.Remove(this);
        if (target.Equals(CombatSystem.instance.allyUnits[0]))
        {
            target.meleeAttackStrength -= (int)(0.25 * (target as Player).initMeleeAttackStrength);
            target.mentalAttackStrength -= (int)(0.25 * (target as Player).initMentalAttackStrength);
        }
        else
        {
            target.meleeAttackStrength -= (int)(0.25 * target.meleeAttackStrength);
            target.mentalAttackStrength -= (int)(0.25 * target.mentalAttackStrength);
        }
    }
}
