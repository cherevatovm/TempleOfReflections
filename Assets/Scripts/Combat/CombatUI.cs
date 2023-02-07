using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public TextMeshProUGUI combatDialogue;
    public GameObject[] buttonPrefabs;
    private List<Button> mentalSkillButtonList = new();
    private bool[] availableMentalSkillButtons = new bool[4];
    [HideInInspector] public bool areButtonsShown;
    [HideInInspector] public bool skillButtonsWereInstantiated;

    public void SetMentalSkillButtons()
    {
        availableMentalSkillButtons[0] = true;
        for (int i = 1; i < availableMentalSkillButtons.Length; i++)
            availableMentalSkillButtons[i] = Inventory.instance.IsMentalParInInventory(i - 1);
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
        for (int i = 1; i < availableMentalSkillButtons.Length; i++)
            if (!availableMentalSkillButtons[i])
                mentalSkillButtonList[i].gameObject.SetActive(false);
        areButtonsShown = true;
    }

    public void HideOrShowMentalSkillButtons()
    {
        if (mentalSkillButtonList[0].gameObject.activeSelf)
        {
            for (int i = 0; i < mentalSkillButtonList.Count; i++)
                if (availableMentalSkillButtons[i])
                    mentalSkillButtonList[i].gameObject.SetActive(false);
            areButtonsShown = false;
        }
        else
        {
            for (int i = 0; i < mentalSkillButtonList.Count; i++)
                if (availableMentalSkillButtons[i])
                    mentalSkillButtonList[i].gameObject.SetActive(true);
            areButtonsShown = true;
        }
    }

    public void UpdateMentalSkillButtons()
    {
        for (int i = 1; i < availableMentalSkillButtons.Length; i++)
            availableMentalSkillButtons[i] = Inventory.instance.IsMentalParInInventory(i - 1);
    }
}
