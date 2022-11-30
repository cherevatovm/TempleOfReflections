using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public static ItemInfo instance;
    Text descriptionText;   
    Button button;
    InventorySlot slot;
    

    void Start()
    {
        descriptionText = gameObject.transform.GetChild(1).GetComponent<Text>();
        instance = this;
        button = gameObject.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(delegate { slot.DropOutOfSlot(); });
    }

    public void Open(string description, Vector3 pos, InventorySlot slot)
    {
        descriptionText.text = description;
        gameObject.transform.localScale = Vector3.one;
        var vector = new Vector3(pos.x + 5, pos.y + 2, pos.z);
        gameObject.transform.position = vector;
        this.slot = slot;
    }

    public void Close() => gameObject.transform.localScale = Vector3.zero;
}
