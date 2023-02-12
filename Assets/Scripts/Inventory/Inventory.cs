using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    [SerializeField] Transform parentSlotForItems;
    [SerializeField] Transform parentSlotForParasites;
    [SerializeField] Transform parentContainerSlots;

    List<InventorySlot> inventorySlotsForItems = new();
    List<InventorySlot> inventorySlotsForParasites = new();
    private List<ContainerSlot> inventoryContainerSlots = new();

    public Unit attachedUnit;

    public static Inventory instance;
    public bool isOpen;
    
    [HideInInspector] public Container container;
    [HideInInspector] public bool isContainerOpen;

    void Start()
    {
        instance = this;
        for (int i = 0; i < parentSlotForItems.childCount; i++)
            inventorySlotsForItems.Add(parentSlotForItems.GetChild(i).GetComponent<InventorySlot>());
        for (int i = 0; i < parentSlotForParasites.childCount; i++)
            inventorySlotsForParasites.Add(parentSlotForParasites.GetChild(i).GetComponent<InventorySlot>());
        for (int i = 0; i < parentContainerSlots.childCount; i++)
            inventoryContainerSlots.Add(parentContainerSlots.GetChild(i).GetComponent<ContainerSlot>());
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isContainerOpen && !CombatSystem.instance.isInCombat && !GameUI.instance.IsSubmenuActive() && !attachedUnit.IsDead())
            if (isOpen)
                instance.Close();
            else
                instance.Open();
    }

    public void Open()
    {
        gameObject.transform.localScale = Vector3.one;
        if (isContainerOpen)
            transform.GetChild(1).gameObject.SetActive(true);
        else
            transform.GetChild(0).gameObject.SetActive(true);
        isOpen = true;
    }

    public void Close()
    {
        gameObject.transform.localScale = Vector3.zero;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        ItemInfo.instance.Close();
        isOpen = false;
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

    bool IsFull(int whichSlots)
    {
        if (whichSlots == 1)
        {
            for (int i = inventorySlotsForParasites.Count - 1; i > -1; i--)
                if (inventorySlotsForParasites[i].isEmpty)
                    return false;
        }
        else if (whichSlots == 0)
        {
            for (int i = inventorySlotsForItems.Count - 1; i > -1; i--)
                if (inventorySlotsForItems[i].isEmpty)
                    return false;
        }
        else
        {
            for (int i = inventoryContainerSlots.Count - 1; i > -1; i--)
                if (inventoryContainerSlots[i].isEmpty)
                    return false;
        }
        return true;
    }

    public void PutInInventory(PickableItem item, GameObject obj)
    {
        if (item.isParasite)
        {
            if (IsFull(1))
            {
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
            GameUI.instance.SetUI(attachedUnit);
        }
        else
        {
            if (IsFull(0))
            {
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

    public void PutInContainer(InventorySlot slot)
    {
        if (IsFull(2))
            return;
        for (int i = 0; i < inventoryContainerSlots.Count; i++)
        {
            if (!inventoryContainerSlots[i].isEmpty && slot.slotItemName.Equals(inventoryContainerSlots[i].slotItemName))
            {
                inventoryContainerSlots[i].stackCount++;
                container.containerSlots[i].stackCount++;
                break;
            }
            else if (inventoryContainerSlots[i].isEmpty)
            {
                inventoryContainerSlots[i].PutInSlot(slot.slotItem, slot.slotObject);
                container.containerSlots[i].PutInSlot(slot.slotItem, slot.slotObject);
                break;
            }
        }
        slot.DropOutOfSlot();
    }

    public void ClearSameIndexSlotInContainer(ContainerSlot slot)
    {
        for (int i = 0; i < inventoryContainerSlots.Count; i++)
            if (slot.Equals(inventoryContainerSlots[i]))
            {
                if (container.containerSlots[i].stackCount != 1)
                {
                    container.containerSlots[i].stackCount--;
                    if (container.containerSlots[i].stackCount == 1)
                        container.containerSlots[i].stackCountText.text = "";
                    else
                        container.containerSlots[i].stackCountText.text = container.containerSlots[i].stackCount.ToString();
                }
                else
                    container.containerSlots[i].Clear();
                break;
            }
    }
}
