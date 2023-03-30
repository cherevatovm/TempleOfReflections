using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurificationTalisman : PickableItem
{
    private void Start()
    {
        isUsableInCombatOnly = true;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        if (!CombatSystem.instance.playerUnit.appliedEffect.Contains(true))
        {
            message = CombatSystem.instance.playerUnit.unitName + " не имеет активных эффектов";
            return;
        }
        CombatSystem.instance.playerUnit.appliedEffect[System.Array.FindIndex(CombatSystem.instance.playerUnit.appliedEffect, elem => elem.Equals(true))] = false;
        CombatSystem.instance.playerUnit.underEffectTurnsCount = 0;
        message = CombatSystem.instance.playerUnit.unitName + " использует талисман очищения";
    }
}