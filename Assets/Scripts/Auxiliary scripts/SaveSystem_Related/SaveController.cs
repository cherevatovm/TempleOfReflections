using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Transform itemParent;
    [SerializeField] private Transform containerParent;
    [SerializeField] private Transform merchantParent;
    [SerializeField] private Transform enemyParent;
    public int currentObelisk;
    public static SaveController instance;

    private void Start() => instance = this;

    public InventoryData GetInventoryData()
    {
        List<SerialTuple<int, int>> items = new();
        foreach (InventorySlot slot in Inventory.instance.inventorySlotsForItems)
        {
            if (slot.isEmpty)
                break;
            items.Add(new SerialTuple<int, int>(slot.slotItem.itemID, slot.stackCount));
        }
        List<SerialTuple<int, int>> parasites = new();
        foreach (InventorySlot slot in Inventory.instance.inventorySlotsForParasites)
        {
            if (slot.isEmpty)
                break;
            Parasite par = slot.slotItem as Parasite;
            parasites.Add(new SerialTuple<int, int>(par.posEffectIndex, par.negEffectIndex));
        }
        return new InventoryData(Inventory.instance.containerKeysInPossession, Inventory.instance.doorKeysInPossession, 
            Inventory.instance.coinsInPossession, Inventory.instance.shardsInPossession, items, parasites);
    }

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
            List<SerialTuple<int, int>> itemsInContainer = new();
            List<ContainerSlot> containerSlots = containerParent.GetChild(i).GetComponent<Container>().containerSlots;
            foreach (ContainerSlot slot in containerSlots)
            {
                if (slot.isEmpty)
                    break;
                itemsInContainer.Add(new SerialTuple<int, int>(slot.slotItem.itemID, slot.stackCount));
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
            List<SerialTuple<int, int>> merchantsItems = new();
            List<ContainerSlot> tradingSlots = merchantParent.GetChild(i).GetComponent<Merchant>().tradingSlots;
            foreach (ContainerSlot slot in tradingSlots)
            {
                if (slot.isEmpty)
                    break;
                merchantsItems.Add(new SerialTuple<int, int>(slot.slotItem.itemID, slot.stackCount));
            }
            res.Add(new MerchantData(merchantParent.GetChild(i).GetComponent<Merchant>().coinsInPossession, merchantsItems));
        }
        return res;
    }

    public List<int> GetSlainEnemyList()
    {
        List<int> res = new();
        for (int i = 0; i < enemyParent.childCount; i++)
        {
            if (!enemyParent.GetChild(i).gameObject.activeSelf)
                res.Add(i);
        }
        return res;
    }

    public void DestroyItems()
    {
        foreach (Transform item in itemParent)
            Destroy(item.gameObject);
    }

    private void LoadInventory()
    {
        InventoryData inData = GameController.instance.receivedSaveData.inventoryData;
        Inventory.instance.ChangeContKeyAmount(inData.containerKeysInPossession);
        Inventory.instance.doorKeysInPossession = inData.doorKeysInPossession;
        Inventory.instance.ChangeCoinAmount(false, inData.coinsInPossession);
        Inventory.instance.ChangeShardAmount(inData.shardsInPossession);
        Inventory.instance.attachedUnit.armorModifier -= 0.1f * inData.shardsInPossession;
        foreach (var tuple in inData.items)
            Inventory.instance.PutInInventory(Instantiate(GameController.instance.prefabs[tuple.first]), tuple.second);
        foreach (var tuple in inData.parasites)
        {
            Parasite par = Instantiate(GameController.instance.prefabs[^1]).GetComponent<Parasite>();
            par.posEffectIndex = tuple.first;
            par.negEffectIndex = tuple.second;
            par.ChangeDescription(tuple.first, tuple.second);
            Inventory.instance.PutInInventory(par.gameObject);
        }
    }

    private void LoadEnemies()
    {
        foreach (int slainEnemyID in GameController.instance.receivedSaveData.slainEnemies)
            enemyParent.transform.GetChild(slainEnemyID).gameObject.SetActive(false);
    }
}
