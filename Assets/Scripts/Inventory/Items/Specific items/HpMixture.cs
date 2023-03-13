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
            CombatSystem.instance.playerUnit.Heal((int)(CombatSystem.instance.playerUnit.maxHP * (percentOfRestoredHP / 100.0)));
            message = CombatSystem.instance.playerUnit.unitName + " использует целебную микстуру";
        }
        else
        {
            Inventory.instance.attachedUnit.Heal((int)(Inventory.instance.attachedUnit.maxHP * (percentOfRestoredHP / 100.0)));
            message = string.Empty;
        }
    }
}
