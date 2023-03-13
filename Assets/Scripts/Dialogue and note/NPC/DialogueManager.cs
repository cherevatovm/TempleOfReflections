using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager instance;
	public TMP_Text nameText;
	public TMP_Text dialogueText;
	[SerializeField] GameObject dialogueUI;
	[SerializeField] PlayerMovement playerMovement;
    [HideInInspector] public DialogueTrigger dialogueTrigger;

    private Queue<string> sentences;

	private void Start()
	{
		instance = this;
		sentences = new Queue<string>();
	}

	public void StartDialogue(Dialogue dialogue)
	{
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        else if (GameUI.instance.exitUI.activeSelf)
            GameUI.instance.exitUI.SetActive(false);
        dialogueTrigger.wasKeyPressed = false;
        dialogueTrigger.pressLock = true;
        playerMovement.enabled = false;
		nameText.text = dialogue.name;
		sentences.Clear();
		foreach (string sentence in dialogue.sentences)
			sentences.Enqueue(sentence);
		dialogueUI.SetActive(true);
		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}
		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	private IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return new WaitForSeconds(0.03f);
		}
	}

	private void EndDialogue()
	{
		playerMovement.enabled = true;
        dialogueUI.SetActive(false);
        dialogueTrigger.pressLock = false;
    }
}