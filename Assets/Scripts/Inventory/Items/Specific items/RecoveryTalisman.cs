using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryTalisman : ItemWithEffect
{
    private void Start()
    {
        underEffectTurnsNumber = 8;
        isUsableInCombatOnly = true;
        doesHaveContinuousEffect = true;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.tempAllyID];
        if (target.affectingItems.Exists(item => item is RecoveryTalisman))
            return;
        target.affectingItems.Add(this);
        message = $"{target.unitName} ощущает эффект талисмана восстановления";
    }

    public override void ApplyEffect()
    {
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        int restoredHP = (int)(target.maxHP * 0.1);
        int restoredMP = (int)(target.maxMP * 0.1);
        target.Heal(restoredHP);
        target.IncreaseCurrentMP(restoredMP);
        target.combatHUD.ChangeHP(target.currentHP);
        target.combatHUD.ChangeMP(target.currentMP);
        CombatSystem.instance.combatUI.combatDialogue.text = $"{target.unitName} восполняет {restoredHP} здоровья и {restoredMP} MP";
    }

    public override void RemoveEffect() => CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].affectingItems.Remove(this);
}