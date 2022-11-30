using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Transform parentSlotForItems;
    [SerializeField] Transform parentSlotForParasites;
    [SerializeField] Rigidbody2D rb;

    List<InventorySlot> inventorySlotsForItems = new();
    List<InventorySlot> inventorySlotsForParasites = new();
    
    public static Inventory instance;
    bool isOpened;


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
        if (Input.GetKeyDown(KeyCode.B))
            if (isOpened)
                instance.Close();
            else
                instance.Open();
    }

    void Open()
    {
        gameObject.transform.localScale = Vector3.one;
        isOpened = true;
    }

    void Close()
    {
        gameObject.transform.localScale = Vector3.zero;
        ItemInfo.instance.Close();
        isOpened = false;
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
        Debug.Log(obj.name);
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
                    inventorySlotsForParasites[i].stackCount++;
                    break;
                }
                else if (inventorySlotsForParasites[i].isEmpty)
                {
                    inventorySlotsForParasites[i].PutInSlot(item, obj, rb);
                    break;
                }
            }
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
                    inventorySlotsForItems[i].PutInSlot(item, obj, rb);
                    break;
                }
            }
        }
    }
}
