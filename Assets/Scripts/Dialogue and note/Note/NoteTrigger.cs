using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTrigger : DialogueTrigger
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (wasKeyPressed)
        {
            NoteManager.instance.dialogueTrigger = this;
            NoteManager.instance.StartReading(dialogue1);
        }
    }
}
