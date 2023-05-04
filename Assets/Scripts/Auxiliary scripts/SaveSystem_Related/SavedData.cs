using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SavedData
{
    public int currentSceneIndex;
    public int trackCurrentlyPlaying;
    public List<EnemyInfoPanel.EnemyRecord> enemyRecords;
    
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    public int currentIdol;

    public InventoryData inventoryData;
    public List<ItemData> itemData;
    public List<ParasiteData> parasiteData;
    public List<ContainerData> containerData;
    public List<MerchantData> merchantData;
    public List<bool> alreadyTalkedToNPCs;
    public List<bool> doorData;
    public List<bool> destEventData;
    public List<int> spawnedUnits;
    public List<int> slainEnemies;
    

    public SavedData(Player player, InventoryData inventoryData, int currentSceneIndex, int currentIdol = -1,
        List<ItemData> itemData = null, List<ParasiteData> parasiteData = null, List<ContainerData> containerData = null, 
        List<MerchantData> merchantData = null, List<bool> alreadyTalkedToNPCs = null, List<bool> doorData = null, 
        List<bool> destEventData = null, List<int> spawnedUnits = null, List<int> slainEnemies = null)
    {
        if (currentSceneIndex < 0)
            this.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        else
            this.currentSceneIndex = currentSceneIndex;
        if (this.currentSceneIndex == 1 || this.currentSceneIndex == 2)
            trackCurrentlyPlaying = 12;
        enemyRecords = EnemyInfoPanel.instance.enemyRecords;
        currentHP = player.currentHP;
        maxHP = player.maxHP;
        currentMP = player.currentMP;
        maxMP = player.maxMP;
        this.currentIdol = currentIdol;
        this.inventoryData = inventoryData;
        this.itemData = itemData;
        this.parasiteData = parasiteData;
        this.containerData = containerData;
        this.merchantData = merchantData;
        this.alreadyTalkedToNPCs = alreadyTalkedToNPCs;
        this.doorData = doorData;
        this.destEventData = destEventData;
        this.spawnedUnits = spawnedUnits;
        this.slainEnemies = slainEnemies;
    }
}

[System.Serializable]
public struct SerialTuple<T1, T2>
{
    public T1 first;
    public T2 second;
    public T2 third;

    public SerialTuple(T1 first, T2 second)
    {
        this.first = first;
        this.second = second;
        third = default;
    }

    public SerialTuple(T1 first, T2 second, T2 third)
    {
        this.first = first;
        this.second = second;
        this.third = third;
    }
} 

[System.Serializable]
public class InventoryData
{
    public int containerKeysInPossession;
    public int doorKeysInPossession;
    public int coinsInPossession;
    public int shardsInPossession;
    public List<SerialTuple<int, int>> items;
    public List<SerialTuple<int, int>> parasites;

    public InventoryData(int containerKeys, int doorKeys, int coins, int shards, List<SerialTuple<int, int>> items, List<SerialTuple<int, int>> parasites)
    {
        containerKeysInPossession = containerKeys;
        doorKeysInPossession = doorKeys;
        coinsInPossession = coins;
        shardsInPossession = shards;
        this.items = items;
        this.parasites = parasites;
    }
}

[System.Serializable]
public class ItemData
{
    public float positionX;
    public float positionY;
    public int itemID;

    public ItemData(float positionX, float positionY, int itemID)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.itemID = itemID;
    }
}

[System.Serializable]
public class ParasiteData : ItemData
{
    public int posEffectIndex;
    public int negEffectIndex;
    public int probability;

    public ParasiteData(float positionX, float positionY, int itemID, int posEffectIndex, int negEffectIndex, int probability): 
        base(positionX, positionY, itemID)
    {
        this.posEffectIndex = posEffectIndex;
        this.negEffectIndex = negEffectIndex;
        this.probability = probability;
    }
}

[System.Serializable]
public class ContainerData 
{
    public bool isNeedOfKey;
    public List<SerialTuple<int, int>> itemsInContainer;

    public ContainerData(bool isNeedOfKey, List<SerialTuple<int, int>> itemsInContainer)
    {
        this.isNeedOfKey = isNeedOfKey;
        this.itemsInContainer = itemsInContainer;
    }
}

[System.Serializable]
public class MerchantData
{
    public int coinsInPossession;
    public List<SerialTuple<int, int>> merchantsItems;
    
    public MerchantData(int coinsInPossession, List<SerialTuple<int, int>> merchantsItems)
    {
        this.coinsInPossession = coinsInPossession;
        this.merchantsItems = merchantsItems;
    }
}