using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Transform parentSlotForItems;
    [SerializeField] Transform parentSlotForParasites;
    [SerializeField] Transform parentContainerSlots;

    private List<InventorySlot> inventorySlotsForItems = new();
    private List<InventorySlot> inventorySlotsForParasites = new();
    private List<ContainerSlot> inventoryContainerSlots = new();

    public Player attachedUnit;

    public static Inventory instance;
    [HideInInspector] public bool isOpen;
    public int KeyCount;
    
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

    public void PutInInventory(GameObject obj)
    {
        PickableItem item = obj.GetComponent<PickableItem>();
        if (item is Key)
        {
            KeyCount++;
            Destroy(obj);
        }
        else if (item.isParasite)
        {
            if (IsFull(1))
                return;
            for (int i = 0; i < inventorySlotsForParasites.Count; i++)
            {
                if (inventorySlotsForParasites[i].isEmpty)
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
            for (int i = 0; i < inventorySlotsForItems.Count; i++)
            {
                if (!inventorySlotsForItems[i].isEmpty && item.itemName.Equals(inventorySlotsForItems[i].slotItemName))
                {
                    inventorySlotsForItems[i].stackCount++;
                    Destroy(obj);
                    break;
                }
                else if (inventorySlotsForItems[i].isEmpty)
                {
                    inventorySlotsForItems[i].PutInSlot(item, obj);
                    obj.SetActive(false);
                    break;
                }
            }            
        }
    }

    public void GroupParasitesInSlots()
    {
        for (int i = 0; i < inventorySlotsForParasites.Count - 1; i++)
        {
            if (inventorySlotsForParasites[i].isEmpty && !inventorySlotsForParasites[i + 1].isEmpty)
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
        for (int i = 0; i < inventorySlotsForItems.Count - 1; i++)
        {
            if (inventorySlotsForItems[i].isEmpty && !inventorySlotsForItems[i + 1].isEmpty)
            {
                int j = i;
                int k = i + 1;
                while (k != inventorySlotsForItems.Count && !inventorySlotsForItems[k].isEmpty)
                {
                    inventorySlotsForItems[j].PutInSlot(inventorySlotsForItems[k].slotItem, inventorySlotsForItems[k].slotObject);
                    if (inventorySlotsForItems[k].stackCount > 1)
                        inventorySlotsForItems[j].stackCount = inventorySlotsForItems[k].stackCount;
                    inventorySlotsForItems[k].Clear();
                    k++;
                    j++;
                }
                break;
            }
        }
    }

    public void GroupItemsInContainerSlots()
    {
        for (int i = 0; i < inventoryContainerSlots.Count - 1; i++)
        {
            if (inventoryContainerSlots[i].isEmpty && !inventoryContainerSlots[i + 1].isEmpty)
            {
                int j = i;
                int k = i + 1;
                while (k != inventoryContainerSlots.Count && !inventoryContainerSlots[k].isEmpty)
                {
                    inventoryContainerSlots[j].PutInSlot(inventoryContainerSlots[k].slotItem, inventoryContainerSlots[k].slotObject);
                    container.containerSlots[j].PutInSlot(inventoryContainerSlots[k].slotItem, inventoryContainerSlots[k].slotObject);
                    if (inventoryContainerSlots[k].stackCount > 1)
                    {
                        inventoryContainerSlots[j].stackCount = inventoryContainerSlots[k].stackCount;
                        container.containerSlots[j].stackCount = inventoryContainerSlots[k].stackCount;
                    }
                    inventoryContainerSlots[k].Clear();
                    container.containerSlots[k].Clear();
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

    private bool IsFull(int whichSlots)
    {
        if (whichSlots == 0)
            return !inventorySlotsForItems[^1].isEmpty;
        else if (whichSlots == 1)
            return !inventorySlotsForParasites[^1].isEmpty;
        else
            return !inventoryContainerSlots[^1].isEmpty;
    }

    public void PutInContainer(InventorySlot slot)
    {
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
                GameObject containerObject = Instantiate(slot.slotObject);
                inventoryContainerSlots[i].PutInSlot(containerObject.GetComponent<PickableItem>(), containerObject);
                container.containerSlots[i].PutInSlot(containerObject.GetComponent<PickableItem>(), containerObject);
                containerObject.SetActive(false);
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
