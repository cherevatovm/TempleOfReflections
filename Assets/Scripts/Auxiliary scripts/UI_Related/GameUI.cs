using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TMP_Text inventoryDialogue;
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

    public void OpenItemPanel(PickableItem pickableItem = null)
    {
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        else if (exitUI.activeSelf)
            ShowOrHideExitUI();
        gameDialogue.text = string.Empty;
        itemPanel.SetActive(true);
        if (pickableItem == null)
            return;
        if (GameController.instance.isInTutorial)
        {
            if (pickableItem is Parasite)
            {
                itemPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = pickableItem.itemDescription;
                ItemPanelTutorialMode(0);
            }
            else if (pickableItem is Key)
                ItemPanelTutorialMode(1, 1);
            else
                ItemPanelTutorialMode(1);
        }
        else
            itemPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = pickableItem.itemDescription;
    }

    public void ItemPanelTutorialMode(int tutorialIndex, int tutorialPartIndex = 0)
    {
        itemPanel.transform.GetChild(0).gameObject.SetActive(false);
        Transform tutorialVer = itemPanel.transform.GetChild(1);
        tutorialVer.gameObject.SetActive(true);
        Button button = tutorialVer.GetChild(2).GetComponent<Button>();
        button.onClick.RemoveAllListeners();       
        if (tutorialIndex == 0)
        {
            tutorialVer.GetChild(0).GetComponent<Text>().text = "Об осколках";           
            if (tutorialPartIndex == 0)
            {
                tutorialVer.GetChild(1).GetComponent<Text>().text = "Туториал об осколках 1";
                button.onClick.AddListener(SwitchFromTutorialVersion);
            }
            else
            {
                itemPanel.SetActive(true);
                tutorialVer.GetChild(1).GetComponent<Text>().text = "Туториал об осколках 2";
                button.onClick.AddListener(CloseItemPanel);
            }
        }
        else if (tutorialIndex == 1)
        {
            if (tutorialPartIndex == 0)
            {
                tutorialVer.GetChild(0).GetComponent<Text>().text = "О предметах";
                tutorialVer.GetChild(1).GetComponent<Text>().text = "Туториал о предметах";
            }
            else
            {
                tutorialVer.GetChild(0).GetComponent<Text>().text = "О ключах";
                tutorialVer.GetChild(1).GetComponent<Text>().text = "Туториал о ключах";
            }
            button.onClick.AddListener(CloseItemPanel);
        }
        else if (tutorialIndex == 2)
        {
            tutorialVer.GetChild(0).GetComponent<Text>().text = "О торговцах";
            tutorialVer.GetChild(1).GetComponent<Text>().text = "Туториал о торговцах";
            button.onClick.AddListener(StartDialogueAfter);
        }
        else if (tutorialIndex == 3)
        {
            tutorialVer.GetChild(0).GetComponent<Text>().text = "О сундуках";
            if (tutorialPartIndex == 0)
                tutorialVer.GetChild(1).GetComponent<Text>().text = "Туториал о сундуках 1";
            else
                tutorialVer.GetChild(1).GetComponent<Text>().text = "Туториал о сундуках 2";
            button.onClick.AddListener(OpenChest);
        }
    }

    private void SwitchFromTutorialVersion()
    {
        itemPanel.transform.GetChild(1).gameObject.SetActive(false);
        itemPanel.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void StartDialogueAfter()
    {
        DialogueTrigger dt = DialogueManager.instance.dialogueTrigger;
        DialogueManager.instance.StartDialogue(dt.dialogue1);
        dt.alreadyTalkedTo = true;
    }

    private void OpenChest()
    {
        CloseItemPanel();
        Inventory.instance.Open();
        GameController.instance.wasContainerTutorialShown = true;
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
