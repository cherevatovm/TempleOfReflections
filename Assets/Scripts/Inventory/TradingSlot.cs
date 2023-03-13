using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingSlot : ContainerSlot
{
    public override void DropOutOfSlot()
    {
        if (Inventory.instance.coinsInPossession >= slotItem.itemValue)
        {
            base.DropOutOfSlot();
            Inventory.instance.coinsInPossession -= slotItem.itemValue;
        }
        else
            GameUI.instance.gameDialogue.text = "У вас недостаточно монет, чтобы купить этот предмет";
    }
}
