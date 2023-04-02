using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PickableItem
{
    public int amount;

    public override void UseItem(out string message) => message = string.Empty;
}
