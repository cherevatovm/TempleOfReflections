using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public Dialogue dialogue1;
	public Dialogue dialogue2;
	bool wasKeyPressed;
	bool alreadyTalkedTo;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
			wasKeyPressed = true;
    }

    void OnTriggerStay2D(Collider2D collision)
	{
		if (wasKeyPressed)
		{
			if (!alreadyTalkedTo)
                DialogueManager.instance.StartDialogue(dialogue1);
			else
                DialogueManager.instance.StartDialogue(dialogue2);
            alreadyTalkedTo = true;
            wasKeyPressed = false;
        }
	}
}
