using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Transform itemParent;
    [SerializeField] private Transform containerParent;
    [SerializeField] private Transform merchantParent;

    public int currentObelisk;
    public static SaveController instance;

    private void Start() => instance = this;

    public List<ItemData> GetItemDataList()
    {
        if (itemParent.childCount == 0)
            return null;
        List<ItemData> res = new();
        for (int i = 0; i < itemParent.childCount; i++)
        {
            Transform item = itemParent.GetChild(i);
            if (item.gameObject.activeSelf)           
                res.Add(new ItemData(item.position.x, item.position.y, item.position.z, item.GetComponent<PickableItem>().itemID));
        }
        return res;
    }

    public List<ContainerData> GetContainerDataList()
    {
        List<ContainerData> res = new();
        for (int i = 0; i < containerParent.childCount; i++)
        {
            List<(int, int)> itemsInContainer = new();
            List<ContainerSlot> containerSlots = containerParent.GetChild(i).GetComponent<Container>().containerSlots;
            foreach (ContainerSlot slot in containerSlots)
            {
                if (slot.isEmpty)
                    break;
                itemsInContainer.Add((slot.slotItem.itemID, slot.stackCount));
            }
            res.Add(new ContainerData(containerParent.GetChild(i).GetComponent<Container>().GetIsNeedOfKey(), itemsInContainer));
        }
        return res;
    }

    public List<MerchantData> GetMerchantDataList() 
    {
        List<MerchantData> res = new();
        for (int i = 0; i < merchantParent.childCount; i++)
        {
            List<(int, int)> merchantsItems = new();
            List<ContainerSlot> tradingSlots = merchantParent.GetChild(i).GetComponent<Merchant>().tradingSlots;
            foreach (ContainerSlot slot in tradingSlots)
            {
                if (slot.isEmpty)
                    break;
                merchantsItems.Add((slot.slotItem.itemID, slot.stackCount));
            }
            res.Add(new MerchantData(merchantParent.GetChild(i).GetComponent<Merchant>().coinsInPossession, merchantsItems));
        }
        return res;
    }

    public void DestroyItems()
    {
        foreach (Transform item in itemParent)
            Destroy(item.gameObject);
    }
}
