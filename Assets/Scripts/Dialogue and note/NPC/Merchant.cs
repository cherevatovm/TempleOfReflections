using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : DialogueTrigger
{
    [SerializeField] private Transform parentSlots;
    [HideInInspector] public List<ContainerSlot> tradingSlots = new(16);
    public Dialogue tradingDialogue;
    public int coinsInPossession;

    private void Awake()
    {
        for (int i = 0; i < parentSlots.childCount; i++)
            tradingSlots.Add(parentSlots.GetChild(i).GetComponent<TradingSlot>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && inTriggerArea && !pressLock)
        {
            DialogueManager.instance.dialogueTrigger = this;
            if (GameController.instance.isInTutorial && !GameController.instance.inventoryTutorialSteps[0])
            {
                Inventory.instance.PutInInventory(Instantiate(GameController.instance.prefabs[3]));
                GameUI.instance.ItemPanelTutorialMode(2);
                GameUI.instance.OpenItemPanel();
                GameUI.instance.gameDialogue.text = string.Empty;
            }
            else
            {
                if (!alreadyTalkedTo)
                    DialogueManager.instance.StartDialogue(dialogue1);
                else
                    DialogueManager.instance.StartDialogue(dialogue2);
                alreadyTalkedTo = true;
            }
        }
    }
}
