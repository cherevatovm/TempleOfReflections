using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakeningTalisman : ItemWithEffect
{
    private void Start()
    {
        underEffectTurnsNumber = 3;
        isUsableInCombatOnly = true;
        isAffectingEnemy = true;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        if (currentEnemyUnit.affectingItem is WeakeningTalisman)
            return;
        currentEnemyUnit.underItemEffect = true;
        currentEnemyUnit.affectingItem = this;
        currentEnemyUnit.meleeAttackStrength -= (int)(0.25 * currentEnemyUnit.meleeAttackStrength);
        currentEnemyUnit.mentalAttackStrength -= (int)(0.25 * currentEnemyUnit.mentalAttackStrength);
        message = CombatSystem.instance.playerUnit.unitName + " использует талисман ослабления для " + currentEnemyUnit.unitName;
    }

    public override void RemoveEffect()
    {
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        if (CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID].itemEffectTurnsCount != underEffectTurnsNumber)
            return;
        currentEnemyUnit.underItemEffect = false;
        currentEnemyUnit.itemEffectTurnsCount = 0;
        currentEnemyUnit.affectingItem = null;
        currentEnemyUnit.meleeAttackStrength += (int)(0.25 * currentEnemyUnit.meleeAttackStrength);
        currentEnemyUnit.mentalAttackStrength += (int)(0.25 * currentEnemyUnit.mentalAttackStrength);
    }
}