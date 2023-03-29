using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{  
    private List<Button> mentalSkillButtonList = new();
    public TextMeshProUGUI combatDialogue;
    public GameObject[] blackouts;
    public GameObject[] buttonPrefabs;
    [HideInInspector] public bool areButtonsShown;
    [HideInInspector] public bool skillButtonsWereInstantiated;

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
        for (int i = 0; i < buttonPrefabs.Length; i++)
        {
            mentalSkillButtonList.Add(Instantiate(buttonPrefabs[i], transform).GetComponent<Button>());
            switch (mentalSkillButtonList[i].name)
            {
                case "Psiona Button(Clone)":
                    mentalSkillButtonList[i].onClick.AddListener(delegate { GameObject.Find("Combat System").GetComponent<CombatSystem>().OnPsionaButton(); });
                    break;
                case "Electra Button(Clone)":
                    mentalSkillButtonList[i].onClick.AddListener(delegate { GameObject.Find("Combat System").GetComponent<CombatSystem>().OnElectraButton(); });
                    break;
                case "Fira Button(Clone)":
                    mentalSkillButtonList[i].onClick.AddListener(delegate { GameObject.Find("Combat System").GetComponent<CombatSystem>().OnFiraButton(); });
                    break;
                case "Regena Button(Clone)":
                    mentalSkillButtonList[i].onClick.AddListener(delegate { GameObject.Find("Combat System").GetComponent<CombatSystem>().OnRegenaButton(); });
                    break;
            }
        }
        bool[] skillArr = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].availableMentalSkills;
        for (int i = 0; i < skillArr.Length; i++)
            if (!skillArr[i])
                mentalSkillButtonList[i].gameObject.SetActive(false);
        areButtonsShown = true;
    }

    public void HideOrShowMentalSkillButtons()
    {        
        if (mentalSkillButtonList.Exists(button => button.gameObject.activeSelf))
        {
            for (int i = 0; i < mentalSkillButtonList.Count; i++)
                mentalSkillButtonList[i].gameObject.SetActive(false);
            areButtonsShown = false;
        }
        else
        {
            bool[] skillArr = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].availableMentalSkills;
            for (int i = 0; i < skillArr.Length; i++)
                if (skillArr[i])
                    mentalSkillButtonList[i].gameObject.SetActive(true);
            areButtonsShown = true;
        }
    }
}
