using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidifiedShard : PickableItem
{
    public override void UseItem(out string message)
    {
        message = string.Empty;
        Inventory.instance.attachedUnit.armorModifier = Inventory.instance.attachedUnit.armorModifier - 0.1f;
    }
}