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
        if (CombatSystem.instance.playerUnit.affectingItem is MotivationTalisman)
            return;
        CombatSystem.instance.playerUnit.underItemEffect = true;
        CombatSystem.instance.playerUnit.affectingItem = this;
        CombatSystem.instance.playerUnit.meleeAttackStrength += (int)(0.25 * CombatSystem.instance.playerUnit.initMeleeAttackStrength);
        CombatSystem.instance.playerUnit.mentalAttackStrength += (int)(0.25 * CombatSystem.instance.playerUnit.initMentalAttackStrength);
        message = CombatSystem.instance.playerUnit.unitName + " использует талисман мотивации";
    }

    public override void RemoveEffect()
    {
        if (CombatSystem.instance.playerUnit.itemEffectTurnsCount != underEffectTurnsNumber)
            return;
        CombatSystem.instance.playerUnit.underItemEffect = false;
        CombatSystem.instance.playerUnit.itemEffectTurnsCount = 0;
        CombatSystem.instance.playerUnit.affectingItem = null;
        CombatSystem.instance.playerUnit.meleeAttackStrength -= (int)(0.25 * CombatSystem.instance.playerUnit.initMeleeAttackStrength);
        CombatSystem.instance.playerUnit.mentalAttackStrength -= (int)(0.25 * CombatSystem.instance.playerUnit.initMentalAttackStrength);
    }
}
