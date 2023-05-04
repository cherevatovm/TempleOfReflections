using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{    
    [SerializeField] private TMP_Text hpCounter;
    [SerializeField] private TMP_Text mpCounter;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider mpSlider; 
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private NoteTrigger aboutControlsNote;
    public GameObject noteUI;
    public GameObject exitUI;
    public GameObject blackout;
    public TMP_Text gameDialogue;
    public static GameUI instance;

    private void Awake()
    { 
        instance = this;
        exitUI.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { SaveSystem.Load(); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowOrHideExitUI();
    }

    public void SetUI(Unit unit)
    {
        hpCounter.text = unit.currentHP + "/" + unit.maxHP;
        mpCounter.text = unit.currentMP + "/" + unit.maxMP;
        hpSlider.maxValue = unit.maxHP;
        mpSlider.maxValue = unit.maxMP;
        hpSlider.value = unit.currentHP;
        mpSlider.value = unit.currentMP;
    }

    public void ChangeHP(int hp)
    {
        if (hp < 0)
            hp = 0;
        hpSlider.value = hp;
        hpCounter.text = hp + "/" + hpSlider.maxValue;
    }

    public void ChangeMP(int mp)
    {
        mpSlider.value = mp;
        mpCounter.text = mp + "/" + mpSlider.maxValue;
    }

    public void ShowOrHideExitUI()
    {
        if (Inventory.instance.isOpen)
        {
            if (Inventory.instance.isInTrade)
                DialogueManager.instance.MerchantsLine();
            else if (Inventory.instance.isContainerOpen)
                Inventory.instance.container.Close();
            else
                Inventory.instance.Close();
            return;
        }
        else if (itemPanel.activeSelf)
        {
            CloseItemPanel();
            return;
        }
        else if (noteUI.activeSelf)
        {
            NoteManager.instance.EndReading();
            return;
        }
        else if (dialogueUI.activeSelf)
            return;
        exitUI.SetActive(!exitUI.activeSelf);
        blackout.SetActive(!blackout.activeSelf);
    }

    public void OpenItemPanel(PickableItem pickableItem)
    {
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        else if (exitUI.activeSelf)
            ShowOrHideExitUI();
        itemPanel.SetActive(true);
        itemPanel.transform.GetChild(1).GetComponent<Text>().text = pickableItem.itemDescription;
    }

    public void CloseItemPanel() => itemPanel.SetActive(false);

    public bool IsSubmenuActive() => noteUI.activeSelf || dialogueUI.activeSelf || itemPanel.activeSelf;

    public void ShowAboutControlsNote()
    {
        NoteManager.instance.dialogueTrigger = aboutControlsNote;
        NoteManager.instance.StartReading(aboutControlsNote.dialogue1);
    }

    public void ShowDeathscreen() => transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);

    public void ExitGame() => Application.Quit();
}
