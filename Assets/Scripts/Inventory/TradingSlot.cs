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
            stackCountText.text = "";
        }
    }

    public override void DropOutOfSlot()
    {
        if (Inventory.instance.coinsInPossession >= slotItem.itemValue)
        {
            Inventory.instance.ChangeCoinAmount(-slotItem.itemValue);
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
