using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTrigger : DialogueTrigger
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && inTriggerArea && !pressLock)
        {
            NoteManager.instance.dialogueTrigger = this;
            NoteManager.instance.StartReading(dialogue1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inTriggerArea = true;
        GameUI.instance.gameDialogue.text = "Нажмите F, чтобы прочитать";
    }

}
