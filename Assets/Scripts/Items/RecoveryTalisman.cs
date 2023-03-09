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
        if (CombatSystem.instance.playerUnit.affectingItem is RecoveryTalisman)
            return;
        CombatSystem.instance.playerUnit.underItemEffect = true;
        CombatSystem.instance.playerUnit.affectingItem = this;
        CombatSystem.instance.playerUnit.Heal((int)(CombatSystem.instance.playerUnit.maxHP * 0.05));
        CombatSystem.instance.playerUnit.IncreaseCurrentMP((int)(CombatSystem.instance.playerUnit.maxMP * 0.05));
        message = CombatSystem.instance.playerUnit.unitName + " использует талисман восстановления";
    }

    public override void RemoveEffect()
    {
        if ((CombatSystem.instance.playerUnit.itemEffectTurnsCount != underEffectTurnsNumber) && (!CombatSystem.instance.playerUnit.isKnockedDown))
            return;
        CombatSystem.instance.playerUnit.underItemEffect = false;
        CombatSystem.instance.playerUnit.itemEffectTurnsCount = 0;
        CombatSystem.instance.playerUnit.affectingItem = null;
    }
}