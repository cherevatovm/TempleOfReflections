using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    private class Pair
    {
        public Vector3 position { get; set; }
        public bool isOccupied { get; set; }

        public Pair(Vector3 position, bool isOccupied)
        {
            this.position = position;
            this.isOccupied = isOccupied;
        }
    }
    
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Text tutorialTitle;
    [SerializeField] private Text tutorialText;
    [SerializeField] private Button tutorialButton;
    [TextArea(3, 10)]
    [SerializeField] private string[] tutorialLines1;
    [TextArea(3, 10)]
    [SerializeField] private string[] tutorialLines2;
    [TextArea(3, 10)]
    [SerializeField] private string[] tutorialLines3;
    private int currentLine;

    private List<Pair> buttonPositions = new();
    [SerializeField] private Button[] mentalSkillButtonList;
    [SerializeField] private Button[] mainButtonList;
    public TextMeshProUGUI combatDialogue;
    public GameObject[] blackouts;
    [HideInInspector] public bool areButtonsShown;
    [HideInInspector] public bool skillButtonsHaveBeenSet;

    private void Start() 
    {
        tutorialButton.onClick.AddListener(CloseTutorialPanel);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EnemyInfoPanel.instance.CloseEnemyInfoPanel();
            if (Inventory.instance.isOpen)
            {
                Inventory.instance.Close();
                combatDialogue.text = "Выберите действие (" + CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].unitName + " ходит в данный момент)";
            }
        }
    }

    public void SetMentalSkillButtons()
    {
        for (int i = 0; i < mentalSkillButtonList.Length; i++)
        {
            switch (i)
            {
                case 0:
                    mentalSkillButtonList[i].onClick.AddListener(delegate { CombatSystem.instance.OnPsionaButton(); });
                    break;
                case 1:
                    mentalSkillButtonList[i].onClick.AddListener(delegate { CombatSystem.instance.OnElectraButton(); });
                    break;
                case 2:
                    mentalSkillButtonList[i].onClick.AddListener(delegate { CombatSystem.instance.OnFiraButton(); });
                    break;
                case 3:
                    mentalSkillButtonList[i].onClick.AddListener(delegate { CombatSystem.instance.OnRegenaButton(); });
                    break;
            }
            buttonPositions.Add(new Pair(mentalSkillButtonList[i].transform.position, false));
            if (CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].availableMentalSkills[i])
            {
                mentalSkillButtonList[i].gameObject.SetActive(true);
                buttonPositions[i].isOccupied = true;
            }
        }
        for (int i = 1; i < mentalSkillButtonList.Length; i++)
        {
            if (mentalSkillButtonList[i].gameObject.activeSelf && !buttonPositions[i - 1].isOccupied)
            {
                int vacantPos = buttonPositions.FindIndex(pos => !pos.isOccupied);
                mentalSkillButtonList[i].transform.position = buttonPositions[vacantPos].position;
                buttonPositions[vacantPos].isOccupied = true;
                buttonPositions[i].isOccupied = false;
            }
        }
        areButtonsShown = true;
    }

    public void HideOrShowMentalSkillButtons()
    {
        if (System.Array.Exists(mentalSkillButtonList, button => button.gameObject.activeSelf))
        {
            for (int i = 0; i < mentalSkillButtonList.Length; i++)
            {
                mentalSkillButtonList[i].gameObject.SetActive(false);
                mentalSkillButtonList[i].transform.position = buttonPositions[i].position;
                buttonPositions[i].isOccupied = false;
            }
            areButtonsShown = false;
        }
        else
        {
            bool[] skillArr = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].availableMentalSkills;
            for (int i = 0; i < skillArr.Length; i++)
            {
                if (skillArr[i])
                {
                    mentalSkillButtonList[i].gameObject.SetActive(true);
                    buttonPositions[i].isOccupied = true;
                }
            }
            for (int i = 1; i < mentalSkillButtonList.Length; i++)
            {
                if (mentalSkillButtonList[i].gameObject.activeSelf && !buttonPositions[i - 1].isOccupied)
                {
                    int vacantPos = buttonPositions.FindIndex(pos => !pos.isOccupied);
                    mentalSkillButtonList[i].transform.position = buttonPositions[vacantPos].position;
                    buttonPositions[vacantPos].isOccupied = true;
                    buttonPositions[i].isOccupied = false;
                }
            }
            areButtonsShown = true;
        }
    }

    public void OpenTutorialPanel()
    {
        switch (GameController.instance.combatTutorialSteps)
        {
            case 0:
                tutorialTitle.text = "Введение";
                tutorialText.text = "Боевая система представлена в пошаговом виде. Во время каждого шага каждый юнит может совершить только одно действие. " +
                    "Начнем с самого элементарного действия - обычной физической атаки, которая не требует траты очков MP. Для ее использования нажмите на кнопку 'Атака' и выберите цель, просто кликнув по врагу левой кнопкой мыши.";
                MakeButtonsNoninteractableExcept(1);
                break;
            case 1:
                currentLine = 0;
                tutorialTitle.text = "О разных действиях";
                tutorialText.text = tutorialLines1[currentLine];
                MakeButtonsNoninteractableExcept(0);
                tutorialButton.onClick.RemoveAllListeners();
                tutorialButton.onClick.AddListener(NextLine);
                break;
            case 2:
                currentLine = 0;
                tutorialTitle.text = "О типах урона";
                tutorialText.text = tutorialLines2[currentLine];
                MakeButtonsNoninteractableExcept();
                tutorialButton.onClick.RemoveAllListeners();
                tutorialButton.onClick.AddListener(NextLine);
                break;
            case 3:
                tutorialTitle.text = "О ментальных навыках";
                tutorialText.text = "Настало время воспользоваться ментальным навыком! Для их использования необходимо тратить очки MP (использование каждого стоит по 3 MP), " +
                    "но они позволяют применять различные типы урона или даже лечить союзников. По умолчанию игроку доступен навык Псиона, " +
                    "наносящий псионический урон. Вы можете открыть больше, если найдете осколки, дающие доступ к навыкам. Для использования навыка нажмите на кнопку 'Ментал', выберите нужный навык, а затем цель для его использования.";
                MakeButtonsNoninteractableExcept(3);
                break;
            case 4:
                currentLine = 0;
                tutorialTitle.text = "Об эффектах";
                tutorialText.text = tutorialLines3[currentLine];
                MakeAllButtonsInteractable();
                tutorialButton.onClick.RemoveAllListeners();
                tutorialButton.onClick.AddListener(NextLine);
                break;
        }
        GameController.instance.combatTutorialSteps++;
        tutorialPanel.SetActive(true);
        blackouts[1].SetActive(true);
    }

    private void MakeButtonsNoninteractableExcept(int buttonInd = -1)
    {
        foreach (Button b in mainButtonList)
                b.interactable = false;
        if (buttonInd != -1)
            mainButtonList[buttonInd].interactable = true;
    }

    private void MakeAllButtonsInteractable()
    {
        foreach (Button b in mainButtonList)
            b.interactable = true;
    }

    private void NextLine()
    {
        string[] lines = null;
        if (GameController.instance.combatTutorialSteps - 1 == 1)
            lines = tutorialLines1;
        else if (GameController.instance.combatTutorialSteps - 1 == 2)
            lines = tutorialLines2;
        else if (GameController.instance.combatTutorialSteps - 1 == 4)
            lines = tutorialLines3;
        tutorialText.text = lines[++currentLine];
        if (currentLine == lines.Length - 1)
        {
            tutorialButton.onClick.RemoveAllListeners();
            tutorialButton.onClick.AddListener(CloseTutorialPanel);
        }
    }
    
    public void CloseTutorialPanel()
    { 
        tutorialPanel.SetActive(false);
        blackouts[1].SetActive(false);
    }
}