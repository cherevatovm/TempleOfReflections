using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image previewImage;
    public string slotItemName;
    public string slotItemDescription;
    public Text stackCountText;
    public PickableItem slotItem;

    public GameObject slotObject;
    protected Button clickableSlot;

    public int stackCount;
    [HideInInspector] public int justBoughtCount;
    [HideInInspector] public bool isEmpty = true;

    private void Start()
    {
        clickableSlot = gameObject.GetComponent<Button>();
        clickableSlot.onClick.AddListener(SlotClicked);
        previewImage = transform.GetChild(0).GetComponent<Image>();
        stackCountText = transform.GetChild(1).GetComponent<Text>();
        stackCount = 1;
        stackCountText.text = "";
    }

    private void Update()
    {
        if (stackCount > 1)
            stackCountText.text = stackCount.ToString();
    }

    public void PutInSlot(PickableItem item, GameObject obj)
    {
        isEmpty = false;
        slotItem = item;
        previewImage.sprite = item.gameObject.GetComponent<SpriteRenderer>().sprite;
        slotItemName = item.itemName;
        slotItemDescription = item.itemDescription;
        slotObject = obj;
    }

    public virtual void DropOutOfSlot()
    {
        if (CombatSystem.instance.isInCombat)
        {
            CombatSystem.instance.combatUI.combatDialogue.text = "Вы не можете выбросить предмет во время боя";
            return;
        }
        var vector = new Vector3(PlayerMovement.instance.transform.position.x + 1.5f, PlayerMovement.instance.transform.position.y, PlayerMovement.instance.transform.position.z);
        ItemInfo.instance.Close();
        if (stackCount != 1)
        {
            if (!(Inventory.instance.isContainerOpen || Inventory.instance.isInTrade))
            {
                slotObject.SetActive(true);
                if (slotItem is Parasite)
                {
                    slotObject.GetComponent<Parasite>().DetachParasite();
                    GameUI.instance.SetUI(Inventory.instance.attachedUnit);
                }
                else
                    Instantiate(slotObject, vector, Quaternion.identity);
                slotObject.SetActive(false);
            }
            stackCount--;
            if (stackCount == 1)
                stackCountText.text = "";
            else
                stackCountText.text = stackCount.ToString();
        }
        else
        {
            if (!(Inventory.instance.isContainerOpen || Inventory.instance.isInTrade))
            {
                slotObject.SetActive(true);
                slotObject.transform.position = vector;
                if (slotItem is Parasite)
                {
                    slotObject.GetComponent<Parasite>().DetachParasite();
                    GameUI.instance.SetUI(Inventory.instance.attachedUnit);
                    Destroy(slotObject);
                }
            }
            else
                Destroy(slotObject);
            Clear();
            Inventory.instance.GroupParasitesInSlots();
            Inventory.instance.GroupItemsInSlots();
        }
    }

    public void UseItemInSlot()
    {
        if (slotItem is Parasite)
            return;
        if (!CombatSystem.instance.isInCombat && slotItem.isUsableInCombatOnly)
            return;
        if (CombatSystem.instance.isInCombat)
        {
            slotItem.UseItem(out string message);
            if (string.IsNullOrEmpty(message))
            {
                CombatSystem.instance.combatUI.combatDialogue.text = "Эффект от этого предмета все еще действует";
                return;
            }
            CombatSystem.instance.combatUI.combatDialogue.text = message;
        }
        else
            slotItem.UseItem(out _);
        if (stackCount != 1)
        {
            stackCount--;
            if (stackCount == 1)
                stackCountText.text = "";
            else
                stackCountText.text = stackCount.ToString();
        }
        else
        {
            ItemInfo.instance.Close();
            Clear();
            Inventory.instance.GroupItemsInSlots();
        }
        if (CombatSystem.instance.isInCombat)
        {
            Inventory.instance.Close();
            StartCoroutine(CombatSystem.instance.AllyUsingItem());
        }
    }

    public virtual void SlotClicked()
    {
        if (!isEmpty && slotItem.GetComponent<Key>() == null)
        {
            var vector = new Vector3(transform.position.x + 5, transform.position.y + 2, transform.position.z);
            if (ItemInfo.instance.transform.localScale == Vector3.zero)
                ItemInfo.instance.Open(slotItemName, slotItemDescription, transform.position, this);
            else if (ItemInfo.instance.transform.localScale == Vector3.one && ItemInfo.instance.transform.position != vector)
                ItemInfo.instance.Open(slotItemName, slotItemDescription, transform.position, this);
            else
                ItemInfo.instance.Close();
        }
    }

    public void Clear()
    {
        isEmpty = true;
        slotItem = null;
        previewImage.sprite = null;
        slotObject = null;
        justBoughtCount = 0;
        stackCount = 1;
        stackCountText.text = string.Empty;
        slotItemName = string.Empty;
        slotItemDescription = string.Empty;
    }
}
