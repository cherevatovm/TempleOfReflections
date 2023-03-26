using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpMixture : PickableItem
{
    public int percentOfRestoredMP;

    public override void UseItem(out string message)
    {
        if (CombatSystem.instance.isInCombat)
        {
            Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
            target.IncreaseCurrentMP((int)(target.maxMP * (percentOfRestoredMP / 100.0)));
            message = target.unitName + " использует порцию успокоительного";
        }
        else
        {
            Inventory.instance.attachedUnit.IncreaseCurrentMP((int)(Inventory.instance.attachedUnit.maxMP * (percentOfRestoredMP / 100.0)));
            message = string.Empty;
        }
    }
}
