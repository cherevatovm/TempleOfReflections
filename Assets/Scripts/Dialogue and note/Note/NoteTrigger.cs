using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTrigger : DialogueTrigger
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && inTriggerArea && !pressLock)
        {
            NoteManager.instance.dialogueTrigger = this;
            NoteManager.instance.StartReading(dialogue1);
        }
    }
}
