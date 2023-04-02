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
		dialogueUI.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(MerchantsLine);
	}

	public void StartDialogue(Dialogue dialogue)
	{
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        else if (GameUI.instance.exitUI.activeSelf)
            GameUI.instance.ShowOrHideExitUI();
		GameUI.instance.CloseItemPanel();
        GameUI.instance.gameDialogue.text = string.Empty;
        dialogueUI.transform.GetChild(0).GetChild(3).gameObject.SetActive(dialogueTrigger is Merchant);
        dialogueTrigger.pressLock = true;
        playerMovement.enabled = false;
		nameText.text = dialogue.name;
		sentences.Clear();
		foreach (string sentence in dialogue.sentences)
			sentences.Enqueue(sentence);
		dialogueUI.SetActive(true);
		DisplayNextSentence();
	}

	public void MerchantsLine()
	{
        Button tradeButton = dialogueUI.transform.GetChild(0).GetChild(3).GetComponent<Button>();
        if (!Inventory.instance.isInTrade)
		{
			sentences.Clear();
			foreach (string sentence in (dialogueTrigger as Merchant).tradingDialogue.sentences)
				sentences.Enqueue(sentence);
            dialogueUI.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);  
            tradeButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Торговать";
			tradeButton.onClick.RemoveListener(MerchantsLine);
			tradeButton.onClick.AddListener(Inventory.instance.OpenTradingMenu);
        }
		else
		{
			if (sentences.Count == 0)
				sentences.Enqueue((dialogueTrigger as Merchant).tradingDialogue.sentences[1]);
            Inventory.instance.CloseTradingMenu();
            dialogueUI.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        }
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
		if (dialogueTrigger is Merchant)
		{
            Button tradeButton = dialogueUI.transform.GetChild(0).GetChild(3).GetComponent<Button>();
            tradeButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Магазин";
			tradeButton.onClick.RemoveListener(Inventory.instance.OpenTradingMenu);
			tradeButton.onClick.AddListener(MerchantsLine);
            Inventory.instance.ClearTradingMenu();
        }
    }

	public void SetActiveDialogueUI(bool isActive) => dialogueUI.SetActive(isActive);
}