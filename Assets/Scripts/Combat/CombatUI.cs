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
    private List<Pair> buttonPositions = new();
    [SerializeField] private Button[] mentalSkillButtonList;
    public TextMeshProUGUI combatDialogue;
    public GameObject[] blackouts;
    [HideInInspector] public bool areButtonsShown;
    [HideInInspector] public bool skillButtonsHaveBeenSet;

    private void Start() => gameObject.SetActive(false);

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
}