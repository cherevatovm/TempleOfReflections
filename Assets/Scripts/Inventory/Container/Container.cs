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

    private void Start()
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
                        Inventory.instance.containerKeysInPossession--;
                        Inventory.instance.containerKeyCounter.text = Inventory.instance.containerKeysInPossession.ToString();
                        Open();
                        isNeedOfKey = false;
                        GameUI.instance.gameDialogue.text = "Вы использовали ключ, чтобы отпереть замок";
                    }
                    else
                        GameUI.instance.gameDialogue.text = "У вас нет ключей, чтобы отпереть замок";
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
            containerSlotsInInventory.GetChild(i).GetComponent<ContainerSlot>().PutInSlot(containerSlots[i].slotItem, containerSlots[i].slotObject);
            if (containerSlots[i].stackCount > 1)
                containerSlotsInInventory.GetChild(i).GetComponent<ContainerSlot>().stackCount = containerSlots[i].stackCount;
        }
        Inventory.instance.isContainerOpen = true;
        Inventory.instance.container = this;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isCloseToContainer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameUI.instance.gameDialogue.text = string.Empty;
            isCloseToContainer = false;
            if (isOpen)
                Close();
        }
    }
}
