using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerSlot : InventorySlot
{
    public override void DropOutOfSlot()
    {
        ContainerItemInfo.instance.Close();
        GameObject obj = Instantiate(slotObject);
        Inventory.instance.PutInInventory(obj);
        if (obj != null)
            obj.SetActive(false);
        Inventory.instance.ClearSameIndexSlotInContainer(this);
        if (stackCount != 1)
        {
            stackCount--;
            if (stackCount == 1)
                stackCountText.text = "";
            else
                stackCountText.text = stackCount.ToString();
        }
        else
        {
            Destroy(slotObject);
            Clear();
        }
        Inventory.instance.GroupItemsInContainerSlots();    
    }

    public override void SlotClicked()
    {
        if (!isEmpty)
        {
            var vector = new Vector3(transform.position.x + 5, transform.position.y + 2, transform.position.z);
            if (ContainerItemInfo.instance.transform.localScale == Vector3.zero)
                ContainerItemInfo.instance.Open(slotItemName, slotItemDescription, transform.position, this);
            else if (ContainerItemInfo.instance.transform.localScale == Vector3.one && ItemInfo.instance.transform.position != vector)
                ContainerItemInfo.instance.Open(slotItemName, slotItemDescription, transform.position, this);
            else
                ContainerItemInfo.instance.Close();
        }
    }
}
