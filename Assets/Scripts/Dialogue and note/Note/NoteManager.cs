using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    public TMP_Text titleText;
    public TMP_Text noteText;
    [HideInInspector] public DialogueTrigger dialogueTrigger;
    public static NoteManager instance;

    void Start() => instance = this;

    public void StartReading(Dialogue note)
    {
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        else if (GameUI.instance.exitUI.activeSelf)
            GameUI.instance.ShowOrHideExitUI();
        GameUI.instance.CloseItemPanel();
        GameUI.instance.gameDialogue.text = string.Empty;
        dialogueTrigger.pressLock = true;
        playerMovement.enabled = false;
        titleText.text = note.name;
        noteText.text = note.sentences[0];
        GameUI.instance.noteUI.SetActive(true);
        GameUI.instance.blackout.SetActive(true);
    }

    public void EndReading()
    {
        playerMovement.enabled = true;
        GameUI.instance.noteUI.SetActive(false);
        GameUI.instance.blackout.SetActive(false);
        dialogueTrigger.pressLock = false;
    }
}
