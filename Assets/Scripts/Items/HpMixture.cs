using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpMixture : PickableItem
{
    public int percentOfRestoredHP;

    public override void UseItem(out string message)
    {
        if (CombatSystem.instance.isInCombat)
        {
            Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
            target.Heal((int)(target.maxHP * (percentOfRestoredHP / 100.0)));
            message = target.unitName + " использует целебную микстуру";
        }
        else
        {
            Inventory.instance.attachedUnit.Heal((int)(Inventory.instance.attachedUnit.maxHP * (percentOfRestoredHP / 100.0)));
            message = string.Empty;
        }
    }
}
