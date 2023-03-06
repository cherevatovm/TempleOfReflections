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
        if (isCloseToContainer && Input.GetKeyDown(KeyCode.E))
            if (isOpen)
                Close();
            else
            {
                if (isNeedOfKey)
                {
                    if (Inventory.instance.KeyCount != 0)
                    {
                        Inventory.instance.KeyCount--;
                        Open();
                        isNeedOfKey = false;
                        Debug.Log("You have opened the container");
                    }
                    else
                        Debug.Log("You don't have key");
                }
                else
                    Open();
            }
    }

    private void Open()
    {
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
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

    private void Close()
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

    private void OnTriggerEnter2D(Collider2D collision) => isCloseToContainer = true;

    private void OnTriggerExit2D(Collider2D collision)
    {
        isCloseToContainer = false;
        Close();
    }
}
