using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    private Button DropButton;
    private Button UseButton;
    private Button PutInContainerButton;
    private InventorySlot slot;
    protected Text descriptionText;
    protected Text nameText;
    public static ItemInfo instance;

    private void Start()
    {
        nameText = transform.GetChild(0).GetComponent<Text>();
        descriptionText = transform.GetChild(1).GetComponent<Text>();
        instance = this;
        DropButton = transform.GetChild(2).GetComponent<Button>();
        DropButton.onClick.AddListener(delegate { slot.DropOutOfSlot(); });
        UseButton = transform.GetChild(3).GetComponent<Button>();
        UseButton.onClick.AddListener(delegate { OnUseButton(); });
        PutInContainerButton = transform.GetChild(4).GetComponent<Button>();
        PutInContainerButton.onClick.AddListener(delegate { Inventory.instance.PutInContainer(slot); });
    }

    public void Open(string itemName, string description, Vector3 pos, InventorySlot slot)
    {
        ContainerItemInfo.instance.Close();
        if (Inventory.instance.isContainerOpen)
        {
            DropButton.gameObject.SetActive(false);
            UseButton.gameObject.SetActive(false);
            PutInContainerButton.gameObject.SetActive(true);
        }
        else
        {
            DropButton.gameObject.SetActive(true);
            UseButton.gameObject.SetActive(true);
            PutInContainerButton.gameObject.SetActive(false);
        }
        if (slot.slotItem.isParasite)
            UseButton.gameObject.SetActive(false);
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
        if (CombatSystem.instance.isInCombat)
        {
            CombatSystem.instance.activeSlot = slot;
            Inventory.instance.Close();
            if (slot.slotItem.isAffectingEnemy)
            {
                CombatSystem.instance.isChoosingEnemyForItem = true;               
                CombatSystem.instance.combatUI.combatDialogue.text = "Выберите врага, на котором хотите использовать предмет";
            }
            else
            {
                CombatSystem.instance.isChoosingAllyForItem = true;
                CombatSystem.instance.combatUI.combatDialogue.text = "Выберите союзника, на котором хотите использовать предмет";
            }
        }
        else
            slot.UseItemInSlot();
    }
}
