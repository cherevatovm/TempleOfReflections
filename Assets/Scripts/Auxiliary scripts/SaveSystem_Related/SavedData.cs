using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SavedData
{
    public int currentSceneIndex;
    public int currentHP;
    public int currentMP;
    public int currentObelisk;

    public InventoryData inventoryData;
    public List<ItemData> itemData;
    public List<ParasiteData> parasiteData;
    public List<ContainerData> containerData;
    public List<MerchantData> merchantData;
    public List<int> slainEnemies;

    public SavedData(Player player, int currentObelisk, InventoryData inventoryData, 
        List<ItemData> itemData, List<ParasiteData> parasiteData, List<ContainerData> containerData, 
        List<MerchantData> merchantData, List<int> slainEnemies)
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentHP = player.currentHP;
        currentMP = player.currentMP;
        this.currentObelisk = currentObelisk;
        this.inventoryData = inventoryData;
        this.itemData = itemData;
        this.parasiteData = parasiteData;
        this.containerData = containerData;
        this.merchantData = merchantData;
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