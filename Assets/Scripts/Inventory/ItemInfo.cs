using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    private Button dropButton;
    private Button useButton;
    private Button sellOrPutInContainerButton;
    protected Button closeButton;
    private InventorySlot slot;
    protected Text descriptionText;
    protected Text nameText;
    public static ItemInfo instance;

    private void Start()
    {
        nameText = transform.GetChild(0).GetComponent<Text>();
        descriptionText = transform.GetChild(1).GetComponent<Text>();
        instance = this;
        dropButton = transform.GetChild(2).GetComponent<Button>();
        dropButton.onClick.AddListener(delegate { slot.DropOutOfSlot(); });
        useButton = transform.GetChild(3).GetComponent<Button>();
        useButton.onClick.AddListener(delegate { OnUseButton(); });
        sellOrPutInContainerButton = transform.GetChild(4).GetComponent<Button>();
        sellOrPutInContainerButton.onClick.AddListener(delegate { Inventory.instance.PutInContainer(slot); });
        closeButton = transform.GetChild(5).GetComponent<Button>();
        closeButton.onClick.AddListener(delegate { Close(); });
    }

    public void Open(string itemName, string description, Vector3 pos, InventorySlot slot)
    {
        ContainerItemInfo.instance.Close();
        if (Inventory.instance.isContainerOpen || Inventory.instance.isInTrade)
        {
            dropButton.gameObject.SetActive(false);
            useButton.gameObject.SetActive(false);
            sellOrPutInContainerButton.gameObject.SetActive(true);
        }
        else
        {
            dropButton.gameObject.SetActive(true);
            useButton.gameObject.SetActive(true);
            sellOrPutInContainerButton.gameObject.SetActive(false);
        }
        if (slot.slotItem is Parasite)
            useButton.gameObject.SetActive(false);
        descriptionText.text = description;
        nameText.text = itemName;
        transform.localScale = Vector3.one;
        var vector = new Vector3(pos.x + 5, pos.y + 2, pos.z);
        transform.position = vector;
        this.slot = slot;
    }

    public void Close() => transform.localScale = Vector3.zero;

    public void OnUseButton()
    {
        if (slot.slotItem.isAffectingEnemy)
        {
            CombatSystem.instance.isChoosingEnemyForItem = true;
            CombatSystem.instance.activeSlot = slot;
            Inventory.instance.Close();
            CombatSystem.instance.combatUI.combatDialogue.text = "�������� ����, �� ������� ������ ������������ �������";
        }
        else
            slot.UseItemInSlot();
    }
    
    public void ChangeSellOrPutInContainerButtonText(bool isTrading)
    {
        if (isTrading)
            sellOrPutInContainerButton.transform.GetChild(0).GetComponent<Text>().text = "�������";
        else
            sellOrPutInContainerButton.transform.GetChild(0).GetComponent<Text>().text = "��������";
    }
}
