using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] Transform parentSlotForItems;
    [SerializeField] Transform parentSlotForParasites;
    [SerializeField] Transform parentContainerSlots;
    [SerializeField] Transform parentTradingSlots;

    private List<InventorySlot> inventorySlotsForItems = new();
    private List<InventorySlot> inventorySlotsForParasites = new();
    private List<ContainerSlot> inventoryContainerSlots = new();
    private List<ContainerSlot> inventoryTradingSlots = new();

    public Player attachedUnit;
    public Text keyCounter;
    [SerializeField] private Text merchantCointCounter;
    [SerializeField] private Text coinCounter;

    public static Inventory instance;
    [HideInInspector] public bool isOpen;
    public int keysInPossession;
    public int coinsInPossession;

    [HideInInspector] public bool isInTrade;
    [HideInInspector] public Container container;
    [HideInInspector] public bool isContainerOpen;

    private void Start()
    {
        instance = this;
        coinCounter.text = coinsInPossession.ToString();
        for (int i = 0; i < parentSlotForItems.childCount; i++)
            inventorySlotsForItems.Add(parentSlotForItems.GetChild(i).GetComponent<InventorySlot>());
        for (int i = 0; i < parentSlotForParasites.childCount; i++)
            inventorySlotsForParasites.Add(parentSlotForParasites.GetChild(i).GetComponent<InventorySlot>());
        for (int i = 0; i < parentContainerSlots.childCount; i++)
            inventoryContainerSlots.Add(parentContainerSlots.GetChild(i).GetComponent<ContainerSlot>());
        for (int i = 0; i < parentTradingSlots.childCount; i++)
            inventoryTradingSlots.Add(parentTradingSlots.GetChild(i).GetComponent<TradingSlot>());
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isContainerOpen && !isInTrade && !CombatSystem.instance.isInCombat && !GameUI.instance.IsSubmenuActive() && !attachedUnit.IsDead())
            if (isOpen)
                instance.Close();
            else
                instance.Open();
    }

    public void Open()
    {
        gameObject.transform.localScale = Vector3.one;
        if (isContainerOpen)
        {
            ItemInfo.instance.ChangeSellOrPutInContainerButtonText(false);
            ContainerItemInfo.instance.ChangeBuyOrTakeButtonText(false);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (isInTrade)
        {
            ItemInfo.instance.ChangeSellOrPutInContainerButtonText(true);
            ContainerItemInfo.instance.ChangeBuyOrTakeButtonText(true);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        isOpen = true;
    }

    public void OpenTradingMenu()
    {
        DialogueManager.instance.SetActiveDialogueUI(false);
        if (inventoryTradingSlots[0].isEmpty)
        {
            Merchant merchant = DialogueManager.instance.dialogueTrigger as Merchant;
            for (int i = 0; i < inventoryTradingSlots.Count; i++)
            {
                if (merchant.tradingSlots[i].isEmpty)
                    break;
                inventoryTradingSlots[i].PutInSlot(merchant.tradingSlots[i].slotItem, merchant.tradingSlots[i].slotObject);
                if (merchant.tradingSlots[i].stackCount > 1)
                    inventoryTradingSlots[i].stackCount = merchant.tradingSlots[i].stackCount;
            }
            merchantCointCounter.text = merchant.coinsInPossession.ToString();
        }
        isInTrade = true;
        Open();
    }

    public void CloseTradingMenu()
    {
        DialogueManager.instance.SetActiveDialogueUI(true);
        ContainerItemInfo.instance.Close();
        isInTrade = false;
        Close();
    }

    public void ClearTradingMenu()
    {
        for (int i = 0; i < inventoryTradingSlots.Count; i++)
        {
            if (inventoryTradingSlots[i].isEmpty)
                break;
            inventoryTradingSlots[i].Clear();
        }
    }

    public void ChangeCoinAmount(int amount)
    {
        coinsInPossession += amount;
        coinCounter.text = coinsInPossession.ToString();
    }

    public void Close()
    {
        gameObject.transform.localScale = Vector3.zero;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        ItemInfo.instance.Close();
        isOpen = false;
        GameUI.instance.gameDialogue.text = string.Empty;
    }

    public void PutInInventory(GameObject obj)
    {
        PickableItem item = obj.GetComponent<PickableItem>();
        if (item is Key)
        {
            keysInPossession++;
            keyCounter.text = keysInPossession.ToString();
            Destroy(obj);
        }
        else if (item is Parasite)
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

    public void GroupItemsInContainerOrTradingSlots(bool isTrading)
    {
        List<ContainerSlot> inventoryList;
        List<ContainerSlot> secondaryList;
        if (isTrading)
        {
            inventoryList = inventoryTradingSlots;
            secondaryList = (DialogueManager.instance.dialogueTrigger as Merchant).tradingSlots;
        }
        else
        {
            inventoryList = inventoryContainerSlots;
            secondaryList = container.containerSlots;
        }
        for (int i = 0; i < inventoryList.Count - 1; i++)
        {
            if (inventoryList[i].isEmpty && !inventoryList[i + 1].isEmpty)
            {
                int j = i;
                int k = i + 1;
                while (k != inventoryList.Count && !inventoryList[k].isEmpty)
                {
                    inventoryList[j].PutInSlot(inventoryList[k].slotItem, inventoryList[k].slotObject);
                    secondaryList[j].PutInSlot(inventoryList[k].slotItem, inventoryList[k].slotObject);
                    if (inventoryList[k].stackCount > 1)
                    {
                        inventoryList[j].stackCount = inventoryList[k].stackCount;
                        secondaryList[j].stackCount = inventoryList[k].stackCount;
                    }
                    inventoryList[k].Clear();
                    secondaryList[k].Clear();
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
        if (whichSlots == 0)
            return !inventorySlotsForItems[^1].isEmpty;
        else if (whichSlots == 1)
            return !inventorySlotsForParasites[^1].isEmpty;
        else
            return !inventoryContainerSlots[^1].isEmpty;
    }

    public void PutInContainer(InventorySlot slot)
    {
        if (!isInTrade)
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
        }
        else
        {
            Merchant merchant = DialogueManager.instance.dialogueTrigger as Merchant;
            if (merchant.coinsInPossession >= slot.slotItem.itemValue)
            {
                for (int i = 0; i < inventoryTradingSlots.Count; i++)
                {
                    if (!inventoryTradingSlots[i].isEmpty && slot.slotItemName.Equals(inventoryTradingSlots[i].slotItemName))
                    {
                        inventoryTradingSlots[i].stackCount++;
                        merchant.tradingSlots[i].stackCount++;
                        break;
                    }
                    else if (inventoryTradingSlots[i].isEmpty)
                    {
                        GameObject tradingObject = Instantiate(slot.slotObject);
                        inventoryTradingSlots[i].PutInSlot(tradingObject.GetComponent<PickableItem>(), tradingObject);
                        merchant.tradingSlots[i].PutInSlot(tradingObject.GetComponent<PickableItem>(), tradingObject);
                        tradingObject.SetActive(false);
                        break;
                    }
                }
                merchant.coinsInPossession -= slot.slotItem.itemValue;
                merchantCointCounter.text = merchant.coinsInPossession.ToString();
                ChangeCoinAmount(slot.slotItem.itemValue);
                GameUI.instance.gameDialogue.text = "Вы продали " + slot.slotItemName;
            }
            else
            {
                ItemInfo.instance.Close();
                GameUI.instance.gameDialogue.text = "У торговца недостаточно денег для того, чтобы вы могли продать этот предмет";
                return;
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
