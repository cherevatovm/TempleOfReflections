using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurificationTalisman : PickableItem
{
    private void Start() => isUsableInCombatOnly = true;

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.tempAllyID];
        if (!target.appliedEffect.Contains(true))
        {
            message = target.unitName + " не имеет активных негативных эффектов";
            return;
        }
        target.appliedEffect[System.Array.FindIndex(target.appliedEffect, elem => elem.Equals(true))] = false;
        target.underEffectTurnsCount = 0;
        message = target.unitName + " использует талисман очищения";
    }
}