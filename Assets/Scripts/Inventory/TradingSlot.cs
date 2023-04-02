using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingSlot : ContainerSlot
{
    public override void DropOutOfSlot()
    {
        if (Inventory.instance.IsFull(0, slotItem))
        {
            GameUI.instance.gameDialogue.text = "������� �� �������, � ����� ��������� �� ������� ����� ��� " + slotItem.itemName;
            ContainerItemInfo.instance.Close();
            return;
        }
        if (justBoughtCount > 0)
        {
            Inventory.instance.ChangeCoinAmount(true, (int)(slotItem.itemValue * 0.75));
            Inventory.instance.ChangeCoinAmount(false, -(int)(slotItem.itemValue * 0.75));
            GameUI.instance.gameDialogue.text = "�� �������� " + slotItem.itemName;
            base.DropOutOfSlot();
            justBoughtCount--;
        }
        else if (Inventory.instance.coinsInPossession >= slotItem.itemValue)
        {
            Inventory.instance.ChangeCoinAmount(true, slotItem.itemValue);
            Inventory.instance.ChangeCoinAmount(false, -slotItem.itemValue);
            GameUI.instance.gameDialogue.text = "�� ��������� " + slotItem.itemName;
            base.DropOutOfSlot();
        }
        else
        {
            ContainerItemInfo.instance.Close();
            GameUI.instance.gameDialogue.text = "� ��� ������������ �����, ����� ������ ���� �������";
        }
    }
}
