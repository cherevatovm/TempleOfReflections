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
        closeButton = transform.GetChild(3).GetComponent<Button>();
        closeButton.onClick.AddListener(delegate { Close(); });
        priceTag = transform.GetChild(4).gameObject;
        priceText = priceTag.transform.GetChild(1).GetComponent<Text>();
    }

    public void Open(string itemName, string description, Vector3 pos, ContainerSlot containerSlot)
    {
        ItemInfo.instance.Close();
        ChangeBuyOrTakeButtonText(Inventory.instance.isInTrade);
        if (Inventory.instance.isInTrade)
        {
            if (containerSlot.justBoughtCount > 0)
            {
                if (GameController.instance.isInTutorial)
                    buyOrTakeButton.interactable = true;
                priceText.text = ((int)(containerSlot.slotItem.itemValue * 0.75)).ToString();
                buyOrTakeButton.transform.GetChild(0).GetComponent<Text>().text = "Вернуть";
            }
            else
            {
                if (GameController.instance.isInTutorial && containerSlot is TradingSlot)
                    buyOrTakeButton.interactable = !GameController.instance.inventoryTutorialSteps[0] || GameController.instance.inventoryTutorialSteps[2];
                priceText.text = containerSlot.slotItem.itemValue.ToString();
            }
        }
        else
            priceText.text = ((int)(containerSlot.slotItem.itemValue * 0.75)).ToString();
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
