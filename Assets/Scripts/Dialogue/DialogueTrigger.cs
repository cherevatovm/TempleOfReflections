using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public Dialogue dialogue1;
	public Dialogue dialogue2;
	public bool wasKeyPressed;
	bool inTriggerArea;
    public bool pressLock;
    public bool isNote;
    bool alreadyTalkedTo;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !pressLock && inTriggerArea)
			wasKeyPressed = true;
    }

    void OnTriggerStay2D(Collider2D collision)
	{
		if (wasKeyPressed)
		{
			if (isNote)
			{
				NoteManager.instance.dialogueTrigger = this;
				NoteManager.instance.StartReading(dialogue1);
			}
			else
			{
                DialogueManager.instance.dialogueTrigger = this;
                if (!alreadyTalkedTo)
					DialogueManager.instance.StartDialogue(dialogue1);
				else
					DialogueManager.instance.StartDialogue(dialogue2);
				alreadyTalkedTo = true;
			}
        }
	}

    void OnTriggerEnter2D(Collider2D collision) => inTriggerArea = true;

    void OnTriggerExit2D(Collider2D collision) => inTriggerArea = false;
}
