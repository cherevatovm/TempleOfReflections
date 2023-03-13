using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private bool alreadyTalkedTo;
    private bool inTriggerArea;
    public Dialogue dialogue1;
    public Dialogue dialogue2;
    [HideInInspector] public bool wasKeyPressed;
    [HideInInspector] public bool pressLock;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !pressLock && inTriggerArea)
            wasKeyPressed = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (wasKeyPressed)
        {
            DialogueManager.instance.dialogueTrigger = this;
            if (!alreadyTalkedTo)
                DialogueManager.instance.StartDialogue(dialogue1);
            else
                DialogueManager.instance.StartDialogue(dialogue2);
            alreadyTalkedTo = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) => inTriggerArea = true;

    private void OnTriggerExit2D(Collider2D collision) => inTriggerArea = false;
}
