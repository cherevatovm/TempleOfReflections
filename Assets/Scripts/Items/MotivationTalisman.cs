using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotivationTalisman : ItemWithEffect
{
    private void Start()
    {
        underEffectTurnsNumber = 3;
        isUsableInCombatOnly = true;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        if (target.affectingItem is MotivationTalisman)
            return;
        target.underItemEffect = true;
        target.affectingItem = this;
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
        if (target.itemEffectTurnsCount != underEffectTurnsNumber)
            return;
        target.itemEffectTurnsCount = 0;
        target.affectingItem = null;
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
