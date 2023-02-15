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

    public void PutInInventory(PickableItem item, GameObject obj)
    {
        if (item.isParasite)
        {
            if (IsFull(1))
                return;
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
            obj.SetActive(false);
            GameUI.instance.SetUI(attachedUnit);
        }
        else
        {
            if (IsFull(0))
                return;
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
            obj.SetActive(false);
        }
    }

    public void GroupParasitesInSlots()
    {
        for (int i = 0; i < inventorySlotsForParasites.Count; i++)
        {
            if (inventorySlotsForParasites[i].isEmpty && IsSomethingInSlotsAfter(true, i))
            {
                int j = i;
                int k = i + 1;
                while (k != inventorySlotsForParasites.Count && !inventorySlotsForParasites[k].isEmpty)
                {
                    inventorySlotsForParasites[j].PutInSlot(inventorySlotsForParasites[k].slotItem, inventorySlotsForParasites[k].slotObject);
                    inventorySlotsForParasites[k].Clear();
                    k++;
                    j++;
                }
                break;
            }
        }
    }

    public void GroupItemsInSlots()
    {
        for (int i = 0; i < inventorySlotsForItems.Count; i++)
        {
            if (inventorySlotsForItems[i].isEmpty && IsSomethingInSlotsAfter(false, i))
            {
                int j = i;
                int k = i + 1;
                while (k != inventorySlotsForItems.Count && !inventorySlotsForItems[k].isEmpty)
                {
                    inventorySlotsForItems[j].PutInSlot(inventorySlotsForItems[k].slotItem, inventorySlotsForItems[k].slotObject);
                    inventorySlotsForItems[k].Clear();
                    k++;
                    j++;
                }
                break;
            }
        }
    }

    public int OccupiedSlotsCount(bool isParasite)
    {
        int count = 0;
        if (isParasite)
        {
            for (int i = 0; i < inventorySlotsForParasites.Count; i++)
                if (!inventorySlotsForParasites[i].isEmpty)
                    count++;
        }
        else
            for (int i = 0; i < inventorySlotsForItems.Count; i++)
                if (!inventorySlotsForItems[i].isEmpty)
                    count++;
        return count;
    }

    public int SameEffectParCount(int effectIndex, bool posOrNeg)
    {
        int count = 0;
        for (int i = 0; i < inventorySlotsForParasites.Count; i++)
        {
            if (inventorySlotsForParasites[i].isEmpty)
                break;
            if (posOrNeg)
            {
                if (inventorySlotsForParasites[i].slotObject.GetComponent<Parasite>().posEffectIndex == effectIndex)
                    count++;
            }
            else
            {
                if (inventorySlotsForParasites[i].slotObject.GetComponent<Parasite>().negEffectIndex == effectIndex)
                    count++;
            }
        }
        return count;
    }

    public bool IsThereParWithSameEffect(Parasite parasite, int effectIndex, out bool posOrNeg)
    {
        int parIndex = 0;
        for (int i = 0; i < inventorySlotsForParasites.Count; i++)
            if (inventorySlotsForParasites[i].slotItem.Equals(parasite))
            {
                parIndex = i;
                break;
            }
        int posIndex = -1;
        int negIndex = -1;
        for (int i = 0; i < inventorySlotsForParasites.Count; i++)
        {
            if (inventorySlotsForParasites[i].isEmpty)
                break;
            if (i == parIndex)
                continue;
            if (inventorySlotsForParasites[i].slotObject.GetComponent<Parasite>().posEffectIndex == effectIndex)
            {
                posIndex = i;
                continue;
            }
            if (inventorySlotsForParasites[i].slotObject.GetComponent<Parasite>().negEffectIndex == effectIndex)
                negIndex = i;
        }
        posOrNeg = posIndex > negIndex;
        return posIndex != -1 || negIndex != -1;
    }

    public bool IsMentalParInInventory(int mentalSkillID)
    {
        foreach (var parInventorySlot in inventorySlotsForParasites)
        {
            if (parInventorySlot.slotObject == null)
                break;
            if (parInventorySlot.slotObject.GetComponent<Parasite>().availableMentalSkills[mentalSkillID])
                return true;
        }
        return false;
    }

    private bool IsFull(int whichSlots)
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

    private bool IsSomethingInSlotsAfter(bool isParasite, int index)
    {
        if (index == inventorySlotsForParasites.Count - 1)
            return false;
        if (isParasite)
        {
            for (int i = index; i < inventorySlotsForParasites.Count; i++)
                if (!inventorySlotsForParasites[i].isEmpty)
                    return true;
        }
        else
            for (int i = index; i < inventorySlotsForItems.Count; i++)
                if (!inventorySlotsForItems[i].isEmpty)
                    return true;
        return false;
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
