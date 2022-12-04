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
	[SerializeField] Canvas dialogueCanvas;
	[SerializeField] PlayerMovement playerMovement;

    private Queue<string> sentences;

	void Start()
	{
		instance = this;
		sentences = new Queue<string>();
	}

	public void StartDialogue(Dialogue dialogue)
	{
		playerMovement.enabled = false;
		nameText.text = dialogue.name;
		sentences.Clear();
		foreach (string sentence in dialogue.sentences)
			sentences.Enqueue(sentence);
		dialogueCanvas.gameObject.SetActive(true);
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

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return new WaitForSeconds(0.03f);
		}
	}

	void EndDialogue()
	{
		playerMovement.enabled = true;
        dialogueCanvas.gameObject.SetActive(false);
	}
}