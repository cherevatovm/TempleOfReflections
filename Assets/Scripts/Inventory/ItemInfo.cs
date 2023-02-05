using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public static ItemInfo instance;
    Text descriptionText;
    Text nameText;
    public Button DropButton;
    public Button UseButton;
    public Button BoxButton;
    InventorySlot slot;
    public bool BIsOpened;



    void Start()
    {
        descriptionText = gameObject.transform.GetChild(1).GetComponent<Text>();
        nameText = gameObject.transform.GetChild(0).GetComponent<Text>();
        instance = this;
        DropButton = gameObject.transform.GetChild(2).GetComponent<Button>();
        DropButton.onClick.AddListener(delegate { slot.DropOutOfSlot(); });
        UseButton = gameObject.transform.GetChild(3).GetComponent<Button>();
        UseButton.onClick.AddListener(delegate { slot.UseItem(); });
        BoxButton = gameObject.transform.GetChild(4).GetComponent<Button>();
        BoxButton.onClick.AddListener(delegate { slot.PutInBox(); });

    }

    public void Open(string itemName, string description, Vector3 pos, InventorySlot slot)
    {
        if (slot.slotItem.isParasite)
            UseButton.gameObject.SetActive(false);
        else
            UseButton.gameObject.SetActive(true);
        descriptionText.text = description;
        nameText.text = itemName;
        gameObject.transform.localScale = Vector3.one;
        var vector = new Vector3(pos.x + 5, pos.y + 2, pos.z);
        gameObject.transform.position = vector;
        this.slot = slot;
        if (BIsOpened)
        {
            ItemInfo.instance.transform.GetChild(2).gameObject.transform.localScale = Vector3.zero;
            ItemInfo.instance.transform.GetChild(3).gameObject.transform.localScale = Vector3.zero;
            ItemInfo.instance.transform.GetChild(4).gameObject.transform.localScale = Vector3.one;
        }
        else
        {
            ItemInfo.instance.transform.GetChild(2).gameObject.transform.localScale = Vector3.one;
            ItemInfo.instance.transform.GetChild(3).gameObject.transform.localScale = Vector3.one;
            ItemInfo.instance.transform.GetChild(4).gameObject.transform.localScale = Vector3.zero;
        }
    }

    public void Close() => gameObject.transform.localScale = Vector3.zero;

    public void BoxIsOpened(bool isopened)
    {
        BIsOpened = isopened;
    }
}
