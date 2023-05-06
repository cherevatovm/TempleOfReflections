using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] private Transform containerSlotsInInventory;
    [SerializeField] private Transform parentSlots;
    [HideInInspector] public List<ContainerSlot> containerSlots = new(16);

    [SerializeField] private bool isNeedOfKey;
    private bool isOpen;
    private bool isCloseToContainer;

    private void Awake()
    {
        for (int i = 0; i < parentSlots.childCount; i++)
            containerSlots.Add(parentSlots.GetChild(i).GetComponent<ContainerSlot>());
    }

    private void Update()
    {
        if (isCloseToContainer && Input.GetKeyDown(KeyCode.F))
            if (isOpen)
                Close();
            else
            {
                if (GameUI.instance.exitUI.activeSelf)
                    GameUI.instance.ShowOrHideExitUI();
                if (isNeedOfKey)
                {
                    if (Inventory.instance.containerKeysInPossession > 0)
                    {
                        Inventory.instance.ChangeContKeyAmount(-1);
                        Open();
                        isNeedOfKey = false;
                        GameUI.instance.inventoryDialogue.text = "Вы использовали ключ, чтобы отпереть замок";
                    }
                    else
                        GameUI.instance.inventoryDialogue.text = "У вас нет ключей, чтобы отпереть замок";
                }
                else
                    Open();
            }
    }

    private void Open()
    {
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        SoundManager.PlaySound(SoundManager.Sound.OpenContainer);
        for (int i = 0; i < containerSlotsInInventory.childCount; i++)
        {
            if (containerSlots[i].isEmpty)
                break;
            containerSlotsInInventory.GetChild(i).GetComponent<ContainerSlot>().PutInSlot(containerSlots[i].slotItem, containerSlots[i].slotObject, containerSlots[i].stackCount);
        }
        Inventory.instance.isContainerOpen = true;
        Inventory.instance.container = this;
        if (GameController.instance.isInTutorial && !GameController.instance.wasContainerTutorialShown)
        {
            GameUI.instance.ItemPanelTutorialMode(3, Inventory.instance.containerKeysInPossession);
            GameUI.instance.OpenItemPanel();
        }
        else
            Inventory.instance.Open();
        isOpen = true;
    }

    public void Close()
    {
        ContainerItemInfo.instance.Close();
        Inventory.instance.Close();
        Inventory.instance.isContainerOpen = false;
        Inventory.instance.container = null;
        for (int i = 0; i < containerSlotsInInventory.childCount; i++)
        {
            if (containerSlotsInInventory.GetChild(i).GetComponent<ContainerSlot>().isEmpty)
                break;
            containerSlotsInInventory.GetChild(i).GetComponent<ContainerSlot>().Clear();
        }
        isOpen = false;
    }

    public bool GetIsNeedOfKey() => isNeedOfKey;

    public void SetIsNeedOfKey(bool value) => isNeedOfKey = value;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCloseToContainer = true;
            GameUI.instance.gameDialogue.text = "Нажмите F, чтобы открыть сундук";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCloseToContainer = false;
            GameUI.instance.gameDialogue.text = string.Empty;
            if (GameController.instance.isInTutorial)
                GameUI.instance.CloseItemPanel();
            if (isOpen)
                Close();
        }
    }
}
