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
    InventorySlot slot;
    

    void Start()
    {
        descriptionText = gameObject.transform.GetChild(1).GetComponent<Text>();
        nameText = gameObject.transform.GetChild(0).GetComponent<Text>();
        instance = this;
        DropButton = gameObject.transform.GetChild(2).GetComponent<Button>();
        DropButton.onClick.AddListener(delegate { slot.DropOutOfSlot(); });
        UseButton = gameObject.transform.GetChild(3).GetComponent<Button>();
        UseButton.onClick.AddListener(delegate { slot.UseItem(); });
    }




    public void Open(string itemName, string description, Vector3 pos, InventorySlot slot)
    {
        descriptionText.text = description;
        nameText.text = itemName;
        gameObject.transform.localScale = Vector3.one;
        var vector = new Vector3(pos.x + 5, pos.y + 2, pos.z);
        gameObject.transform.position = vector;
        this.slot = slot;
    }

    public void Close() => gameObject.transform.localScale = Vector3.zero;
}
