using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager instance;
    public TMP_Text titleText;
    public TMP_Text noteText;
    [SerializeField] GameObject noteCanvas;
    [SerializeField] PlayerMovement playerMovement;
    [HideInInspector] public DialogueTrigger dialogueTrigger;

    void Start() => instance = this;

    public void StartReading(Dialogue note)
    {
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        else if (GameUI.instance.exitUI.activeSelf)
            GameUI.instance.exitUI.SetActive(false);
        dialogueTrigger.wasKeyPressed = false;
        dialogueTrigger.pressLock = true;
        playerMovement.enabled = false;
        titleText.text = note.name;
        noteText.text = note.sentences[0];
        noteCanvas.SetActive(true);
    }

    public void EndReading()
    {
        playerMovement.enabled = true;
        noteCanvas.SetActive(false);
        dialogueTrigger.pressLock = false;
    }
}
