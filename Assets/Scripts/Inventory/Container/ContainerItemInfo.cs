using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerItemInfo : ItemInfo
{
    private Button buyOrTakeButton;
    private ContainerSlot containerSlot;
    public new static ContainerItemInfo instance;

    private void Start()
    {
        instance = this;
        nameText = transform.GetChild(0).GetComponent<Text>();
        descriptionText = transform.GetChild(1).GetComponent<Text>();
        buyOrTakeButton = transform.GetChild(2).GetComponent<Button>();
        buyOrTakeButton.onClick.AddListener(delegate { containerSlot.DropOutOfSlot(); });
        CloseButton = transform.GetChild(3).GetComponent<Button>();
        CloseButton.onClick.AddListener(delegate { Close(); });
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

    public void ChangeBuyOrTakeButtonText(bool isTrading)
    {
        if (isTrading)
            buyOrTakeButton.transform.GetChild(0).GetComponent<Text>().text = "Купить";
        else
            buyOrTakeButton.transform.GetChild(0).GetComponent<Text>().text = "Взять";
    }
}
