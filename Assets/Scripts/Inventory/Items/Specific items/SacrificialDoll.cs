using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SacrificialDoll : ItemWithEffect
{
    private void Awake() 
    {
        isUsableInCombatOnly = true;
        underEffectTurnsNumber = -1;
    }

    public override void UseItem(out string message)
    {
        message = string.Empty;
        if (!CombatSystem.instance.isInCombat)
            return;
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.tempAllyID];
        target.affectingItems.Add(this);
        message = target.unitName + " устанавливает связь с жертвенной куклой";
    }

    public override void RemoveEffect()
    {
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        target.currentHP = (int)(target.maxHP * 0.15);
        target.combatHUD.ChangeHP(target.currentHP);
        target.affectingItems.Remove(this);
        CombatSystem.instance.combatUI.combatDialogue.text = target.unitName + " остается в живых благодаря жертвенной кукле";
    }
}
