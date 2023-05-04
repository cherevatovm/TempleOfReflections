using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Transform itemParent;
    [SerializeField] private Transform parasiteParent;
    [SerializeField] private Transform containerParent;
    [SerializeField] private Transform NPC_Parent;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform doorParent;
    [SerializeField] private Vector3[] playerSpawnPositions;
    [SerializeField] private UnitSpawnEvent[] unitSpawnEvents;
    [SerializeField] private GameObject[] destructibleEventMarkers;

    public static SaveController instance;

    private void Start()
    {
        instance = this;
        ObjectPool.instance.SetSoundPool();
        if (GameController.instance.isSwitchingScenes)
        {
            LoadPlayer();
            LoadInventory();
            SaveSystem.Save(new SavedData(Inventory.instance.attachedUnit,
                instance.GetInventoryData(false), -1, -1, instance.GetItemDataList(), instance.GetParasiteDataList(),
                instance.GetContainerDataList(), instance.GetMerchantDataList(), instance.GetTalkedToNpcData(),
                instance.GetDoorsData(), instance.GetDestEventsData(),
                instance.GetSpawnedUnitsData(), instance.GetSlainEnemyList()));
            EnemyInfoPanel.instance.enemyRecords = GameController.instance.receivedSaveData.enemyRecords.ToList();
            SoundManager.PlaySound((SoundManager.Sound)GameController.instance.receivedSaveData.trackCurrentlyPlaying);
            GameController.instance.isSwitchingScenes = false;
        }
        else if (GameController.instance.hasBeenLoaded)
        {
            LoadPlayer();               
            LoadItemsAndParasites();
            LoadInventory();
            LoadContainers();            
            LoadSpawnedUnits();
            LoadTalkedToNPCs();
            LoadDoors();
            LoadDestEvents();
            LoadMerchants();           
            LoadEnemies();
            EnemyInfoPanel.instance.enemyRecords = GameController.instance.receivedSaveData.enemyRecords.ToList();            
            SoundManager.PlaySound((SoundManager.Sound)GameController.instance.receivedSaveData.trackCurrentlyPlaying);
        }
    }

    public InventoryData GetInventoryData(bool isSwitchingScenes)
    {
        List<SerialTuple<int, int>> items = new();
        foreach (InventorySlot slot in Inventory.instance.inventorySlotsForItems)
        {
            if (slot.isEmpty)
                break;
            items.Add(new SerialTuple<int, int>(slot.slotItem.itemID, slot.stackCount));
        }
        InventoryData invData;
        List<SerialTuple<int, int>> parasites = new();
        if (!isSwitchingScenes)
        {            
            foreach (InventorySlot slot in Inventory.instance.inventorySlotsForParasites)
            {
                if (slot.isEmpty)
                    break;
                Parasite par = slot.slotItem as Parasite;
                parasites.Add(new SerialTuple<int, int>(par.posEffectIndex, par.negEffectIndex, par.percentage));
            }
            invData = new InventoryData(Inventory.instance.containerKeysInPossession, Inventory.instance.doorKeysInPossession,
            Inventory.instance.coinsInPossession, Inventory.instance.shardsInPossession, items, parasites);
        }
        else
            invData = new InventoryData(0, 0, Inventory.instance.coinsInPossession, 0, items, parasites);
        return invData;
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
                res.Add(new ItemData(item.position.x, item.position.y, item.GetComponent<PickableItem>().itemID));
        }
        return res;
    }

    public List<ParasiteData> GetParasiteDataList()
    {
        if (parasiteParent.childCount == 0)
            return null;
        List<ParasiteData> res = new();
        for (int i = 0; i < parasiteParent.childCount; i++)
        {
            Transform transfom = parasiteParent.GetChild(i);
            if (transfom.gameObject.activeSelf)
            {
                Parasite par = transfom.GetComponent<Parasite>();
                res.Add(new ParasiteData(transfom.position.x, transfom.position.y, par.itemID,
                    par.posEffectIndex, par.negEffectIndex, par.percentage));
            }
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

    public List<bool> GetTalkedToNpcData()
    {
        List<bool> res = new();
        foreach (Transform child in NPC_Parent)
            res.Add(child.GetComponentInChildren<DialogueTrigger>().alreadyTalkedTo);
        return res;
    }

    public List<bool> GetDoorsData()
    {
        List<bool> res = new();
        foreach (Transform child in doorParent)
            res.Add(child.GetComponent<Door>().GetIsOpen());
        return res;
    }

    public List<bool> GetDestEventsData()
    {
        List<bool> res = new();
        foreach (GameObject marker in destructibleEventMarkers)
            res.Add(marker == null);
        return res;
    }

    public List<int> GetSpawnedUnitsData()
    {
        List<int> res = new();
        for (int i = 0; i < unitSpawnEvents.Length; i++)
        {
            if (unitSpawnEvents[i] == null)
                res.Add(i);
        }
        return res;
    }

    public List<MerchantData> GetMerchantDataList() 
    {
        List<MerchantData> res = new();
        for (int i = 0; i < NPC_Parent.childCount; i++)
        {
            if (!NPC_Parent.GetChild(i).TryGetComponent<Merchant>(out var merchant))
                break;
            List<SerialTuple<int, int>> merchantsItems = new();
            List<ContainerSlot> tradingSlots = merchant.tradingSlots;
            foreach (ContainerSlot slot in tradingSlots)
            {
                if (slot.isEmpty)
                    break;
                merchantsItems.Add(new SerialTuple<int, int>(slot.slotItem.itemID, slot.stackCount));
            }
            res.Add(new MerchantData(NPC_Parent.GetChild(i).GetComponent<Merchant>().coinsInPossession, merchantsItems));
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

    private void LoadPlayer()
    {
        Player player = Inventory.instance.attachedUnit;
        SavedData sData = GameController.instance.receivedSaveData;
        player.transform.position = sData.currentIdol == -1 ? playerSpawnPositions[^1] : playerSpawnPositions[sData.currentIdol];
        player.currentHP = sData.currentHP;
        player.currentMP = sData.currentMP;
        if (GameController.instance.isSwitchingScenes)
        {
            player.maxHP = sData.maxHP;
            player.maxMP = sData.maxMP;
        }
    }

    private void LoadInventory()
    {
        InventoryData inData = GameController.instance.receivedSaveData.inventoryData;
        Inventory.instance.ChangeContKeyAmount(inData.containerKeysInPossession);
        Inventory.instance.doorKeysInPossession = inData.doorKeysInPossession;
        Inventory.instance.coinsInPossession = 0;
        Inventory.instance.ChangeCoinAmount(false, inData.coinsInPossession);
        Inventory.instance.ChangeShardAmount(inData.shardsInPossession);
        Inventory.instance.attachedUnit.armorModifier -= 0.1f * inData.shardsInPossession;
        foreach (var tuple in inData.items)
            Inventory.instance.PutInInventory(Instantiate(GameController.instance.prefabs[tuple.first]), tuple.second);
        foreach (var triple in inData.parasites)
        {
            Parasite par = Instantiate(GameController.instance.prefabs[^1]).GetComponent<Parasite>();
            par.posEffectIndex = triple.first;
            par.negEffectIndex = triple.second;
            par.percentage = triple.third;
            par.ChangeDescription(triple.first, triple.second);
            Inventory.instance.PutInInventory(par.gameObject);
        }
    }

    private void LoadContainers()
    {
        var contData = GameController.instance.receivedSaveData.containerData;
        for (int i = 0; i < containerParent.childCount; i++)
        {
            Container container = containerParent.GetChild(i).GetComponent<Container>();
            container.SetIsNeedOfKey(contData[i].isNeedOfKey);
            for (int j = 0; j < contData[i].itemsInContainer.Count; j++)
            {
                container.containerSlots[j].stackCount = 0;
                GameObject obj = Instantiate(GameController.instance.prefabs[contData[i].itemsInContainer[j].first], itemParent);
                container.containerSlots[j].PutInSlot(obj.GetComponent<PickableItem>(), obj, contData[i].itemsInContainer[j].second);
                obj.SetActive(false);
            }
        }
    }

    private void LoadMerchants()
    {
        List<MerchantData> mData = GameController.instance.receivedSaveData.merchantData;
        for (int i = 0; i < NPC_Parent.childCount; i++)
        {
            if (!NPC_Parent.GetChild(i).TryGetComponent<Merchant>(out var merchant))
                break;           
            merchant.coinsInPossession = mData[i].coinsInPossession;
            for (int j = 0; j < mData[i].merchantsItems.Count; j++)
            {
                merchant.tradingSlots[j].stackCount = 0;
                GameObject obj = Instantiate(GameController.instance.prefabs[mData[i].merchantsItems[j].first], itemParent);
                merchant.tradingSlots[j].PutInSlot(obj.GetComponent<PickableItem>(), obj, mData[i].merchantsItems[j].second);
                obj.SetActive(false);
            }
        }
    }

    private void LoadTalkedToNPCs()
    {
        for (int i = 0; i < NPC_Parent.childCount; i++)
        {
            Transform npc = NPC_Parent.GetChild(i);
            DialogueTrigger dt;
            if (npc.childCount > 1)
                dt = npc.GetChild(1).GetComponent<DialogueTrigger>();
            else
                dt = npc.GetComponentInChildren<DialogueTrigger>();
            dt.alreadyTalkedTo = GameController.instance.receivedSaveData.alreadyTalkedToNPCs[i];
        }
    }

    private void LoadDoors()
    {
        List<bool> doorList = GameController.instance.receivedSaveData.doorData;
        for (int i = 0; i < doorList.Count; i++)
        {
            if (doorList[i])
                doorParent.GetChild(i).GetComponent<Door>().Open();
        }
    }

    private void LoadDestEvents()
    {
        List<bool> deData = GameController.instance.receivedSaveData.destEventData;
        for (int i = 0; i < deData.Count; i++)
        {
            if (deData[i])
                Destroy(destructibleEventMarkers[i]);
        }
    }

    private void LoadSpawnedUnits()
    {
        foreach (int ind in GameController.instance.receivedSaveData.spawnedUnits)
            unitSpawnEvents[ind].SpawnPrefab();
    }

    private void LoadItemsAndParasites()
    {
        foreach (Transform item in itemParent)
            Destroy(item.gameObject);
        foreach (Transform par in parasiteParent)
            Destroy(par.gameObject);
        foreach (ItemData item in GameController.instance.receivedSaveData.itemData)
            Instantiate(GameController.instance.prefabs[item.itemID], new Vector3(item.positionX, item.positionY), Quaternion.identity, itemParent);
        foreach (ParasiteData par in GameController.instance.receivedSaveData.parasiteData)
        {
            Parasite parasite = Instantiate(GameController.instance.prefabs[^1], new Vector3(par.positionX, par.positionY), Quaternion.identity, parasiteParent).GetComponent<Parasite>();
            parasite.posEffectIndex = par.posEffectIndex;
            parasite.negEffectIndex = par.negEffectIndex;
            parasite.percentage = par.probability;
            parasite.ChangeDescription(par.posEffectIndex, par.negEffectIndex);
        }
    }

    private void LoadEnemies()
    {
        foreach (int slainEnemyID in GameController.instance.receivedSaveData.slainEnemies)
            enemyParent.transform.GetChild(slainEnemyID).gameObject.SetActive(false);
    }
}
