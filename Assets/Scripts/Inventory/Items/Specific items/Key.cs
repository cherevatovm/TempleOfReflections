using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : PickableItem
{
    public bool isDoorKey;

    public override void UseItem(out string message) => message = string.Empty;
}
