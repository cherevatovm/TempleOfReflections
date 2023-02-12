using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakeningTalisman : ItemWithEffect
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
        if (CombatSystem.instance.enemyUnit.affectingItem is WeakeningTalisman)
            return;
        CombatSystem.instance.enemyUnit.underItemEffect = true;
        CombatSystem.instance.enemyUnit.affectingItem = this;
        CombatSystem.instance.enemyUnit.meleeAttackStrength -= (int)(0.25 * CombatSystem.instance.enemyUnit.meleeAttackStrength);
        CombatSystem.instance.enemyUnit.mentalAttackStrength -= (int)(0.25 * CombatSystem.instance.enemyUnit.mentalAttackStrength);
        message = CombatSystem.instance.playerUnit.unitName + " использует талисман ослабления для " + CombatSystem.instance.enemyUnit.unitName;
    }

    public override void RemoveEffect()
    {
        if (CombatSystem.instance.enemyUnit.itemEffectTurnsCount != underEffectTurnsNumber)
            return;
        CombatSystem.instance.enemyUnit.underItemEffect = false;
        CombatSystem.instance.enemyUnit.itemEffectTurnsCount = 0;
        CombatSystem.instance.enemyUnit.affectingItem = null;
        CombatSystem.instance.enemyUnit.meleeAttackStrength += (int)(0.25 * CombatSystem.instance.enemyUnit.meleeAttackStrength);
        CombatSystem.instance.enemyUnit.mentalAttackStrength += (int)(0.25 * CombatSystem.instance.enemyUnit.mentalAttackStrength);
    }
}