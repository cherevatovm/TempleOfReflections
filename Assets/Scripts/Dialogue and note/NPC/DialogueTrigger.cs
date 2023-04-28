using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{    
    protected bool inTriggerArea;
    public bool alreadyTalkedTo;
    public Dialogue dialogue1;
    public Dialogue dialogue2;
    [HideInInspector] public bool pressLock;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && inTriggerArea && !pressLock)
        {
            DialogueManager.instance.dialogueTrigger = this;
            if (!alreadyTalkedTo)
                DialogueManager.instance.StartDialogue(dialogue1);
            else
                DialogueManager.instance.StartDialogue(dialogue2);
            alreadyTalkedTo = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inTriggerArea = true;
        GameUI.instance.gameDialogue.text = "Нажмите F, чтобы поговорить";
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inTriggerArea = false;
        GameUI.instance.gameDialogue.text = string.Empty;
    }
}
