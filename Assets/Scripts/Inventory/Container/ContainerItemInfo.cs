using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerItemInfo : ItemInfo
{
    private Button TakeButton;
    private ContainerSlot containerSlot;
    public new static ContainerItemInfo instance;

    private void Start()
    {
        instance = this;
        nameText = transform.GetChild(0).GetComponent<Text>();
        descriptionText = transform.GetChild(1).GetComponent<Text>();
        TakeButton = transform.GetChild(2).GetComponent<Button>();
        TakeButton.onClick.AddListener(delegate { containerSlot.DropOutOfSlot(); });
    }

    public void Open(string itemName, string description, Vector3 pos, ContainerSlot containerSlot)
    {
        ItemInfo.instance.Close();
        descriptionText.text = description;
        nameText.text = itemName;
        transform.localScale = Vector3.one;
        var vector = new Vector3(pos.x + 5, pos.y + 2, pos.z);
        transform.position = vector;
        this.containerSlot = containerSlot;
    }
}