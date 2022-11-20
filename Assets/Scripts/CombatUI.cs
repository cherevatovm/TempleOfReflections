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
    List<Button> mentalSkillButtonList = new List<Button>();

    public Unit atackingUnit;
    public Unit defendingUnit;

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
                case "Electro Button(Clone)":
                    mentalSkillButtonList[i].onClick.AddListener(delegate { GameObject.Find("Combat System").GetComponent<CombatSystem>().OnElectroButton(); });
                    break;
                case "Fire Button(Clone)":
                    mentalSkillButtonList[i].onClick.AddListener(delegate { GameObject.Find("Combat System").GetComponent<CombatSystem>().OnFireButton(); });
                    break;
            }
        }
    }

    public void HideOrShowMentalSkillButtons()
    {
        SetMentalSkillButtons();
        if (mentalSkillButtonList[0].gameObject.activeSelf)
            foreach (var button in mentalSkillButtonList)
                button.gameObject.SetActive(false);
        else
            foreach (var button in mentalSkillButtonList)
                button.gameObject.SetActive(true);
    }
}
