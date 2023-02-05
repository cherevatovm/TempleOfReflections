using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxSlots : MonoBehaviour
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
        BoxitemInfo.instance.Close();
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
            Clear();
        }
    }

    public void TakeItem()
    {
        Inventory.instance.PutInInventory(slotItem, slotObject);
    }

    public void SlotClicked()
    {
        if (!isEmpty)
        {
            var vector = new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 2, gameObject.transform.position.z);
            if (BoxitemInfo.instance.transform.localScale == Vector3.zero)
                BoxitemInfo.instance.Open(slotItemName, slotItemDescription, gameObject.transform.position, this);
            else if (BoxitemInfo.instance.transform.localScale == Vector3.one && ItemInfo.instance.transform.position != vector)
                BoxitemInfo.instance.Open(slotItemName, slotItemDescription, gameObject.transform.position, this);
            else
                BoxitemInfo.instance.Close();
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
