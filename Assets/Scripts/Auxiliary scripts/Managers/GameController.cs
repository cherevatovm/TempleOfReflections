using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private DataBetweenScenes scriptableObject;
    [SerializeField] private GameObject[] prefabs;
    private List<(int, int)> currentItems = new();
    [HideInInspector] public bool isInDifferentScene;
    public static GameController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SoundManager.InitSoundTimerDict();
        }
        else
        {
            instance.isInDifferentScene = true;
            Destroy(gameObject);                  
        }
    }

    public void WriteData(List<EnemyInfoPanel.EnemyRecord> enemyRecords)
    {
        Player player = Inventory.instance.attachedUnit;
        scriptableObject.currentHP = player.currentHP;
        scriptableObject.maxHP = player.maxHP;
        scriptableObject.currentMP = player.currentMP;
        scriptableObject.maxMP = player.maxMP;
        scriptableObject.coinsInPossession = Inventory.instance.coinsInPossession;
        scriptableObject.keysInPossession = Inventory.instance.keysInPossession;
        scriptableObject.currentEnemyRecords.Clear();
        foreach (var enemyRecord in enemyRecords)
            scriptableObject.currentEnemyRecords.Add(new EnemyInfoPanel.EnemyRecord(enemyRecord.slainInTotal, enemyRecord.knownAffinities));
    }

    public void UnpackWrittenData()
    {
        Player player = Inventory.instance.attachedUnit;
        player.currentHP = scriptableObject.currentHP;
        player.maxHP = scriptableObject.maxHP;
        player.currentMP = scriptableObject.currentMP;
        player.maxMP = scriptableObject.maxMP;
        Inventory.instance.ChangeCoinAmount(false, scriptableObject.coinsInPossession);
        Inventory.instance.keysInPossession = scriptableObject.keysInPossession;
        Inventory.instance.keyCounter.text = scriptableObject.keysInPossession.ToString();
    }

    public void CopyEnemyRecords(ref List<EnemyInfoPanel.EnemyRecord> dest) => dest = scriptableObject.currentEnemyRecords.ToList();

    public void SaveCurrentItemsData()
    {
        currentItems.Clear();
        foreach (InventorySlot slot in Inventory.instance.inventorySlotsForItems)
        {
            if (slot.isEmpty)
                break;
            currentItems.Add((slot.slotItem.itemID, slot.stackCount));
        }
    }

    public void InstantiateAndAddToInventory()
    {
        for (int i = 0; i < currentItems.Count; i++)
        {
            Inventory.instance.PutInInventory(Instantiate(prefabs[currentItems[i].Item1]));
            Inventory.instance.inventorySlotsForItems[i].stackCount = currentItems[i].Item2;
        }
    }
}
