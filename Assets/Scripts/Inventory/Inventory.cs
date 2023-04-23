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

    [HideInInspector] public List<InventorySlot> inventorySlotsForItems = new();
    [HideInInspector] public List<InventorySlot> inventorySlotsForParasites = new();
    private List<ContainerSlot> inventoryContainerSlots = new();
    private List<ContainerSlot> inventoryTradingSlots = new();

    public Player attachedUnit;
    public Text containerKeyCounter;
    [SerializeField] private Text merchantCointCounter;
    [SerializeField] private Text coinCounter;
    [SerializeField] private Text shardCounter;

    public static Inventory instance;   
    public int containerKeysInPossession;
    public int doorKeysInPossession;
    public int coinsInPossession;
    public int shardsInPossession;

    [HideInInspector] public bool isOpen;
    [HideInInspector] public bool isInTrade;    
    [HideInInspector] public bool isContainerOpen;
    [HideInInspector] public Container container;
    [HideInInspector] public GameObject tempItem;

    private bool isOpenFirstTime = true;

    private void Awake()
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

    }

    /*private void Start()
    {
        if (GameController.instance.isInDifferentScene)
        {
            GameController.instance.UnpackWrittenData();
            GameController.instance.InstantiateAndAddToInventory();
        }
    }*/

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isContainerOpen && !isInTrade && !CombatSystem.instance.isInCombat && !GameUI.instance.IsSubmenuActive() && !attachedUnit.IsDead())
            if (isOpen)
                instance.Close();
            else
            {
                if (GameUI.instance.exitUI.activeSelf)
                    GameUI.instance.ShowOrHideExitUI();
                instance.Open();
            }
    }

    public void Open()
    {
        if (isOpenFirstTime)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            isOpenFirstTime = false;
        }
        gameObject.transform.localScale = Vector3.one;
        if (CombatSystem.instance.isInCombat)
            CombatSystem.instance.combatUI.blackouts[0].SetActive(true);
        else
            GameUI.instance.blackout.SetActive(true);
        if (isContainerOpen)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (isInTrade)
        {
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
                inventoryTradingSlots[i].PutInSlot(merchant.tradingSlots[i].slotItem, merchant.tradingSlots[i].slotObject, merchant.tradingSlots[i].stackCount);
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
        for (int i = 0; i < inventorySlotsForItems.Count; i++)
        {
            if (inventorySlotsForItems[i].isEmpty)
                break;
            inventorySlotsForItems[i].justBoughtCount = 0;
        }
    }

    public void ChangeCoinAmount(bool isMerchant, int amount)
    {
        if (isMerchant)
        {
            (DialogueManager.instance.dialogueTrigger as Merchant).coinsInPossession += amount;
            merchantCointCounter.text = (DialogueManager.instance.dialogueTrigger as Merchant).coinsInPossession.ToString();
        }
        else
        {
            coinsInPossession += amount;
            coinCounter.text = coinsInPossession.ToString();
        }
    }

    public void ChangeContKeyAmount(int amount)
    {
        containerKeysInPossession += amount;
        containerKeyCounter.text = containerKeysInPossession.ToString();
    }

    public void ChangeShardAmount(int amount)
    {
        shardsInPossession += amount;
        shardCounter.text = shardsInPossession.ToString();
    }

    public void Close()
    {
        gameObject.transform.localScale = Vector3.zero;
        if (CombatSystem.instance.isInCombat)
            CombatSystem.instance.combatUI.blackouts[0].SetActive(false);
        else
            GameUI.instance.blackout.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        ItemInfo.instance.Close();
        isOpen = false;
        GameUI.instance.gameDialogue.text = string.Empty;
    }

    public void PutInInventory(GameObject obj, int amount = 1, InventorySlot sourceSlot = null)
    {
        PickableItem item = obj.GetComponent<PickableItem>();
        if (item is Key)
        {
            if ((item as Key).isDoorKey)
                doorKeysInPossession++;
            else
                ChangeContKeyAmount(1);
            Destroy(obj);
        }
        else if (item is Coin)
        {
            ChangeCoinAmount(false, (item as Coin).amount);
            Destroy(obj);
        }
        else if (item is SolidifiedShard)
        {
            if (shardsInPossession == 5)
                return;
            ChangeShardAmount(1);
            item.UseItem(out _);
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
            (item as Parasite).ApplyParasiteEffect();
            obj.SetActive(false);
            GameUI.instance.SetUI(attachedUnit);
        }       
        else
        {            
            for (int i = 0; i < inventorySlotsForItems.Count; i++)
            {
                if (!inventorySlotsForItems[i].isEmpty && item.itemName.Equals(inventorySlotsForItems[i].slotItemName))
                {
                    if (isInTrade && sourceSlot.justBoughtCount == 0)
                        inventorySlotsForItems[i].justBoughtCount++;
                    inventorySlotsForItems[i].PutInSlot(item, obj, amount);                   
                    Destroy(obj);
                    break;
                }
                else if (inventorySlotsForItems[i].isEmpty)
                {
                    if (isInTrade && sourceSlot.justBoughtCount == 0)
                        inventorySlotsForItems[i].justBoughtCount++;
                    inventorySlotsForItems[i].PutInSlot(item, obj, amount);
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
                    inventorySlotsForItems[j].PutInSlot(inventorySlotsForItems[k].slotItem, inventorySlotsForItems[k].slotObject, inventorySlotsForItems[k].stackCount);
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
                    inventoryList[j].PutInSlot(inventoryList[k].slotItem, inventoryList[k].slotObject, inventoryList[k].stackCount);
                    secondaryList[j].PutInSlot(inventoryList[k].slotItem, inventoryList[k].slotObject, inventoryList[k].stackCount);
                    inventoryList[k].Clear();
                    secondaryList[k].Clear();
                    k++;
                    j++;
                }
                break;
            }
        }
    }

    public void ClearSameIndexContainerOrTradingSlot(ContainerSlot slot)
    {
        List<ContainerSlot> inventoryList;
        List<ContainerSlot> secondaryList;
        if (isInTrade)
        {
            inventoryList = inventoryTradingSlots;
            secondaryList = (DialogueManager.instance.dialogueTrigger as Merchant).tradingSlots;
        }
        else
        {
            inventoryList = inventoryContainerSlots;
            secondaryList = container.containerSlots;
        }
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (slot.Equals(inventoryList[i]))
            {
                if (secondaryList[i].stackCount != 1)
                {
                    secondaryList[i].stackCount--;
                    if (secondaryList[i].stackCount == 1)
                        secondaryList[i].stackCountText.text = "";
                    else
                        secondaryList[i].stackCountText.text = secondaryList[i].stackCount.ToString();
                }
                else
                    secondaryList[i].Clear();
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
                if ((inventorySlotsForParasites[i].slotItem as Parasite).posEffectIndex == effectIndex)
                    count++;
            }
            else
            {
                if ((inventorySlotsForParasites[i].slotItem as Parasite).negEffectIndex == effectIndex)
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
            if ((inventorySlotsForParasites[i].slotItem as Parasite).posEffectIndex == effectIndex)
            {
                posIndex = i;
                continue;
            }
            if ((inventorySlotsForParasites[i].slotItem as Parasite).negEffectIndex == effectIndex)
                negIndex = i;
        }
        posOrNeg = posIndex > negIndex;
        return posIndex != -1 || negIndex != -1;
    }

    public bool IsFull(int whichSlots, PickableItem item = null)
    {
        if (whichSlots == 0)
        {
            for (int i = 0; i < inventorySlotsForItems.Count; i++)
            {
                if (inventorySlotsForItems[i].isEmpty || item.itemName.Equals(inventorySlotsForItems[i].slotItemName))
                    return false;
            }
            return true;
        }
        else if (whichSlots == 1)
            return !inventorySlotsForParasites[^1].isEmpty;
        List<ContainerSlot> inventoryList;
        if (whichSlots == 2)
            inventoryList = inventoryContainerSlots;
        else
            inventoryList = inventoryTradingSlots;
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].isEmpty || item.itemName.Equals(inventoryList[i].slotItemName))
                return false;
        }
        return true;
    }

    public void PutInContainer(InventorySlot slot)
    {
        if (!isInTrade)
        {
            for (int i = 0; i < inventoryContainerSlots.Count; i++)
            {
                if (!inventoryContainerSlots[i].isEmpty && slot.slotItemName.Equals(inventoryContainerSlots[i].slotItemName))
                {
                    inventoryContainerSlots[i].PutInSlot(slot.slotItem, slot.slotObject);
                    container.containerSlots[i].PutInSlot(slot.slotItem, slot.slotObject);
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
            if (IsFull(3, slot.slotItem))
            {
                GameUI.instance.gameDialogue.text = "Продажа не удалась, инвентарь торговца полон";
                ItemInfo.instance.Close();
                return;
            }
            Merchant merchant = DialogueManager.instance.dialogueTrigger as Merchant;
            if (merchant.coinsInPossession >= slot.slotItem.itemValue)
            {
                for (int i = 0; i < inventoryTradingSlots.Count; i++)
                {
                    if (!inventoryTradingSlots[i].isEmpty && slot.slotItemName.Equals(inventoryTradingSlots[i].slotItemName))
                    {
                        if (slot.justBoughtCount == 0)
                            inventoryTradingSlots[i].justBoughtCount++;
                        inventoryTradingSlots[i].PutInSlot(slot.slotItem, slot.slotObject);
                        merchant.tradingSlots[i].PutInSlot(slot.slotItem, slot.slotObject);
                        break;
                    }
                    else if (inventoryTradingSlots[i].isEmpty)
                    {
                        GameObject tradingObject = Instantiate(slot.slotObject);
                        inventoryTradingSlots[i].PutInSlot(tradingObject.GetComponent<PickableItem>(), tradingObject);
                        if (slot.justBoughtCount == 0)
                            inventoryTradingSlots[i].justBoughtCount++;
                        merchant.tradingSlots[i].PutInSlot(tradingObject.GetComponent<PickableItem>(), tradingObject);
                        tradingObject.SetActive(false);
                        break;
                    }
                }
                if (slot.justBoughtCount > 0)
                {
                    slot.justBoughtCount--;
                    ChangeCoinAmount(true, -slot.slotItem.itemValue);
                    ChangeCoinAmount(false, slot.slotItem.itemValue);
                    GameUI.instance.gameDialogue.text = "Вы вернули " + slot.slotItemName;
                }
                else
                {
                    ChangeCoinAmount(true, -(int)(slot.slotItem.itemValue * 0.75));
                    ChangeCoinAmount(false, (int)(slot.slotItem.itemValue * 0.75));
                    GameUI.instance.gameDialogue.text = "Вы продали " + slot.slotItemName;
                }
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

    public void PutParasiteInInventory()
    {
        PutInInventory(tempItem);
        tempItem = null;
    }
}
