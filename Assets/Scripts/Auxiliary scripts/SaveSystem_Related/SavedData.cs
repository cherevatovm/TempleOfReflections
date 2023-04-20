using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedData
{
    public int currentSceneIndex;

    public int currentHP;
    public int currentMP;
    public int currentObelisk;

    public List<ItemData> itemData;
    public List<ContainerData> containerData;
    public List<MerchantData> merchantData;
    public List<int> slainEnemies;

    public SavedData(Player player, int currentObelisk)
    {
        currentHP = player.currentHP;
        currentMP = player.currentMP;
        this.currentObelisk = currentObelisk;
    }
}

[System.Serializable]
public class ItemData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public int itemID;

    public ItemData(float positionX, float positionY, float positionZ, int itemID)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.positionZ = positionZ;
        this.itemID = itemID;
    }
}

[System.Serializable]
public class ContainerData 
{
    public bool isNeedOfKey;
    public List<(int, int)> itemsInContainer;

    public ContainerData(bool isNeedOfKey, List<(int, int)> itemsInContainer)
    {
        this.isNeedOfKey = isNeedOfKey;
        this.itemsInContainer = itemsInContainer;
    }
}

[System.Serializable]
public class MerchantData
{
    public int coinsInPossession;
    public List<(int, int)> merchantsItems;
    
    public MerchantData(int coinsInPossession, List<(int, int)> merchantsItems)
    {
        this.coinsInPossession = coinsInPossession;
        this.merchantsItems = merchantsItems;
    }
}