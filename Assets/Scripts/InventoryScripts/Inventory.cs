using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Transform parentSlotForItems;
    [SerializeField] Transform parentSlotForParasites;

    List<InventorySlot> inventorySlotsForItems = new();
    List<InventorySlot> inventorySlotsForParasites = new();

    public Unit attachedUnit;

    public static Inventory instance;
    public bool isOpened;

    void Start()
    {
        instance = this;
        for (int i = 0; i < parentSlotForItems.childCount; i++)
            inventorySlotsForItems.Add(parentSlotForItems.GetChild(i).GetComponent<InventorySlot>());
        for (int i = 0; i < parentSlotForParasites.childCount;i++)
            inventorySlotsForParasites.Add(parentSlotForParasites.GetChild(i).GetComponent<InventorySlot>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) /*&& !CombatSystem.instance.isInCombat*/ && !attachedUnit.IsDead())
            if (isOpened)
                instance.Close();
            else
                instance.Open();
    }

    public void Open()
    {
        Debug.Log("test");
        gameObject.transform.localScale = Vector3.one;
        isOpened = true;
    }

    public void Close()
    {
        gameObject.transform.localScale = Vector3.zero;
        ItemInfo.instance.Close();
        isOpened = false;
    }

    public bool IsElectraParInInventory()
    {
        foreach (var parInventorySlot in inventorySlotsForParasites)
        {
            if (parInventorySlot.slotObject == null)
                break;
            if (parInventorySlot.slotObject.GetComponent<Parasite>().availableDamageTypes[0])
                return true;
        }
        return false;
    }

    public bool IsFiraParInInventory()
    {
        foreach (var parInventorySlot in inventorySlotsForParasites)
        {
            if (parInventorySlot.slotObject == null)
                break;
            if (parInventorySlot.slotObject.GetComponent<Parasite>().availableDamageTypes[1])
                return true;
        }
        return false;
    }

    bool IsFull(bool isParasite)
    {
        if (isParasite)
        {
            for (int i = inventorySlotsForParasites.Count - 1; i > -1; i--)
                if (inventorySlotsForParasites[i].isEmpty)
                    return false;
        }
        else
        {
            for (int i = inventorySlotsForItems.Count - 1; i > -1; i--)
                if (inventorySlotsForItems[i].isEmpty)
                    return false;
        }
        return true;
    }

    public void PutInInventory(PickableItem item, GameObject obj)
    {
        if (item.isParasite)
        {
            if (IsFull(true))
            {
                Debug.Log("Inventory is full");
                return;
            }
            for (int i = 0; i < inventorySlotsForParasites.Count; i++)
            {
                if (!inventorySlotsForParasites[i].isEmpty && item.itemName.Equals(inventorySlotsForParasites[i].slotItemName))
                {
                    Destroy(obj);
                    inventorySlotsForParasites[i].stackCount++;
                    break;
                }
                else if (inventorySlotsForParasites[i].isEmpty)
                {
                    inventorySlotsForParasites[i].PutInSlot(item, obj);
                    break;
                }
            }
            obj.GetComponent<Parasite>().ApplyParasiteEffect();
        }
        else
        {
            if (IsFull(false))
            {
                Debug.Log("Inventory is full");
                return;
            }
            for (int i = 0; i < inventorySlotsForItems.Count; i++)
            {
                if (!inventorySlotsForItems[i].isEmpty && item.itemName.Equals(inventorySlotsForItems[i].slotItemName))
                {
                    inventorySlotsForItems[i].stackCount++;
                    break;
                }
                else if (inventorySlotsForItems[i].isEmpty)
                {
                    inventorySlotsForItems[i].PutInSlot(item, obj);
                    break;
                }
            }
        }
    }
}
