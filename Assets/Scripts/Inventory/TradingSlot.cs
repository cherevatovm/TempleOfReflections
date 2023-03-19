using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingSlot : ContainerSlot
{
    private void Start()
    {
        clickableSlot = gameObject.GetComponent<Button>();
        clickableSlot.onClick.AddListener(SlotClicked);
        previewImage = transform.GetChild(0).GetComponent<Image>();
        stackCountText = transform.GetChild(1).GetComponent<Text>();
        if (slotObject != null)
            PutInSlot(slotObject.GetComponent<PickableItem>(), slotObject);
        else
        {
            stackCount = 1;
            stackCountText.text = string.Empty;
        }
    }

    public override void DropOutOfSlot()
    {
        if (Inventory.instance.IsFull(0, slotItem))
        {
            GameUI.instance.gameDialogue.text = "Покупка не удалась, в вашем инвентаре не нашлось места для " + slotItem.itemName;
            ContainerItemInfo.instance.Close();
            return;
        }
        if (justBoughtCount > 0)
        {
            Inventory.instance.ChangeCoinAmount(true, (int)(slotItem.itemValue * 0.75));
            Inventory.instance.ChangeCoinAmount(false, -(int)(slotItem.itemValue * 0.75));
            GameUI.instance.gameDialogue.text = "Вы выкупили " + slotItem.itemName;
            base.DropOutOfSlot();
            justBoughtCount--;
        }
        else if (Inventory.instance.coinsInPossession >= slotItem.itemValue)
        {
            Inventory.instance.ChangeCoinAmount(true, slotItem.itemValue);
            Inventory.instance.ChangeCoinAmount(false, -slotItem.itemValue);
            GameUI.instance.gameDialogue.text = "Вы приобрели " + slotItem.itemName;
            base.DropOutOfSlot();
        }
        else
        {
            ContainerItemInfo.instance.Close();
            GameUI.instance.gameDialogue.text = "У вас недостаточно монет, чтобы купить этот предмет";
        }
    }
}
