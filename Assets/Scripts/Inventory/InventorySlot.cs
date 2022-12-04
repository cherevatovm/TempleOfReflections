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
    Button clickableSlot;
           
    public int stackCount;
    public bool isEmpty = true;

    void Start()
    {
        clickableSlot = gameObject.GetComponent<Button>();
        clickableSlot.onClick.AddListener(SlotClicked);        
        previewImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        stackCountText = gameObject.transform.GetChild(1).GetComponent<Text>();
        stackCount = 1;
        stackCountText.text = "";
    }

    void Update()
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

    public void DropOutOfSlot()
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
            slotObject.SetActive(true);
            if (slotItem.isParasite)
                slotObject.GetComponent<Parasite>().DetachParasite();
            else
                Instantiate(slotObject, vector, Quaternion.identity);
            slotObject.SetActive(false);
            stackCount--;
            if (stackCount == 1)
                stackCountText.text = "";
            else
                stackCountText.text = stackCount.ToString();
        }
        else
        {
            slotObject.SetActive(true);
            slotObject.transform.position = vector;
            if (slotItem.isParasite)
            {
                slotObject.GetComponent<Parasite>().DetachParasite();
                Destroy(slotObject);
            }
            Clear();
        }
    }

    public void UseItem()
    {
        if (slotItem.isParasite)
            return;
        if (CombatSystem.instance.isInCombat)
            CombatSystem.instance.wasAnItemUsed = true;
        string itemType = slotItem.GetType().ToString();
        switch (itemType)
        {
            case "HpMixture":
                var mixture0 = (HpMixture)slotItem;
                if (CombatSystem.instance.isInCombat)
                    CombatSystem.instance.playerUnit.Heal((int)(CombatSystem.instance.playerUnit.maxHP * (mixture0.percentOfRestoredHP / 100.0)));
                else
                    Inventory.instance.attachedUnit.Heal((int)(Inventory.instance.attachedUnit.maxHP * (mixture0.percentOfRestoredHP / 100.0)));
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
                }
                break;
            case "MpMixture":
                var mixture1 = (MpMixture)slotItem;
                if (CombatSystem.instance.isInCombat)
                    CombatSystem.instance.playerUnit.IncreaseCurrentMP((int)(CombatSystem.instance.playerUnit.maxMP * (mixture1.percentOfRestoredMP / 100.0)));
                else
                    Inventory.instance.attachedUnit.IncreaseCurrentMP((int)(Inventory.instance.attachedUnit.maxMP * (mixture1.percentOfRestoredMP / 100.0)));
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
                }
                break;
        }
        if (CombatSystem.instance.isInCombat)
        {
            Inventory.instance.Close();
            StartCoroutine(CombatSystem.instance.PlayerUsingItem());
        }
    }

    public void SlotClicked() 
    {
        if (!isEmpty)
        { 
            var vector = new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 2, gameObject.transform.position.z);
            if (ItemInfo.instance.transform.localScale == Vector3.zero)
                ItemInfo.instance.Open(slotItemName, slotItemDescription, gameObject.transform.position, this);
            else if (ItemInfo.instance.transform.localScale == Vector3.one && ItemInfo.instance.transform.position != vector)
                ItemInfo.instance.Open(slotItemName, slotItemDescription, gameObject.transform.position, this);
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
        slotItemName = "";
        slotItemDescription = "";
    }
}
