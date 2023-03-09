using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SacrificialDoll : PickableItem
{
    public void Start()
    {
        isUsableInCombatOnly = true;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        CombatSystem.instance.playerUnit.currentHP = (int)(CombatSystem.instance.playerUnit.maxHP * 0.15);
        message = CombatSystem.instance.playerUnit.unitName + " использует жертвенную куклу";
    }
}
