using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public TextMeshProUGUI combatDialogue;
    public GameObject[] buttonPrefabs;
    List<Button> mentalSkillButtonList = new();
    bool[] availableMentalSkillButtons = new bool[3];
    public bool areButtonsShown;
    public bool mSkillButtonsWereinstantiated;

    public void SetMentalSkillButtons()
    {
        availableMentalSkillButtons[0] = true;
        availableMentalSkillButtons[1] = true;//Inventory.instance.IsElectraParInInventory();
        availableMentalSkillButtons[2] = true;//Inventory.instance.IsFiraParInInventory();
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
            }
        }
        for (int i = 1; i < availableMentalSkillButtons.Length; i++)
            if (!availableMentalSkillButtons[i])
                mentalSkillButtonList[i].gameObject.SetActive(false);
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
}
