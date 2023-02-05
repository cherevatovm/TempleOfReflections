using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxitemInfo : MonoBehaviour
{
    public static BoxitemInfo instance;
    Text descriptionText;
    Text nameText;
    BoxSlots slot;
    Button TakeButton;


    void Start()
    {
        descriptionText = gameObject.transform.GetChild(1).GetComponent<Text>();
        nameText = gameObject.transform.GetChild(0).GetComponent<Text>();
        TakeButton = gameObject.transform.GetChild(2).GetComponent<Button>();
        instance = this;
        TakeButton.onClick.AddListener(delegate { slot.TakeItem(); slot.DropOutOfSlot(); });
    }

    public void Open(string itemName, string description, Vector3 pos, BoxSlots slot)
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
