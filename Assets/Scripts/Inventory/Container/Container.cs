using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] private Transform containerSlotsInInventory;
    [SerializeField] private Transform parentSlots;
    [HideInInspector] public List<ContainerSlot> containerSlots = new(16);
    private bool isOpen;
    private bool isCloseToContainer;

    private void Start()
    {
        for (int i = 0; i < parentSlots.childCount; i++)
            containerSlots.Add(parentSlots.GetChild(i).GetComponent<ContainerSlot>());
    }

    private void Update()
    {
        if (isCloseToContainer)
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isOpen)
                    Close();
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
