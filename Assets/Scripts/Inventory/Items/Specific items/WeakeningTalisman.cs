using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakeningTalisman : ItemWithEffect
{
    private void Awake()
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
        if (currentEnemyUnit.affectingItems.Exists(item => item is WeakeningTalisman))
            return;
        currentEnemyUnit.affectingItems.Add(this);
        currentEnemyUnit.meleeAttackStrength -= (int)(0.25 * currentEnemyUnit.meleeAttackStrength);
        currentEnemyUnit.mentalAttackStrength -= (int)(0.25 * currentEnemyUnit.mentalAttackStrength);
        message = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].unitName + " использует талисман ослабления для " + currentEnemyUnit.unitName;
    }

    public override void RemoveEffect()
    {
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        currentEnemyUnit.affectingItems.Remove(this);
        currentEnemyUnit.meleeAttackStrength += (int)(0.25 * currentEnemyUnit.meleeAttackStrength);
        currentEnemyUnit.mentalAttackStrength += (int)(0.25 * currentEnemyUnit.mentalAttackStrength);
    }
}