using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform parentSlotForItems;
    [SerializeField] private Transform parentSlotForParasites;

    private List<InventorySlot> inventorySlotsForItems = new();
    private List<InventorySlot> inventorySlotsForParasites = new();

    public Player attachedUnit;

    public static Inventory instance;
    public bool isOpened;

    void Start()
    {
        instance = this;
        for (int i = 0; i < parentSlotForItems.childCount; i++)
            inventorySlotsForItems.Add(parentSlotForItems.GetChild(i).GetComponent<InventorySlot>());
        for (int i = 0; i < parentSlotForParasites.childCount; i++)
            inventorySlotsForParasites.Add(parentSlotForParasites.GetChild(i).GetComponent<InventorySlot>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !CombatSystem.instance.isInCombat && !GameUI.instance.IsSubmenuActive() && !attachedUnit.IsDead())
            if (isOpened)
                instance.Close();
            else
                instance.Open();
    }

    public void Open()
    {
        gameObject.transform.localScale = Vector3.one;
        isOpened = true;
    }

    public void Close()
    {
        gameObject.transform.localScale = Vector3.zero;
        ItemInfo.instance.Close();
        isOpened = false;
    }

    public void PutInInventory(PickableItem item, GameObject obj)
    {
        if (item.isParasite)
        {
            if (IsFull(true))
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
            if (IsFull(false))
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

    private bool IsFull(bool isParasite)
    {
        if (isParasite)
            return OccupiedSlotsCount(true) == inventorySlotsForParasites.Count;
        else
            return OccupiedSlotsCount(false) == inventorySlotsForItems.Count;
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
}
