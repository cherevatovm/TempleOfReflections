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
    public bool buttonShowLock = false;
    public bool mSkillButtonsWereinstantiated;

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
            }
        }
    }

    public void HideOrShowMentalSkillButtons()
    {
        if (mentalSkillButtonList[0].gameObject.activeSelf)
            foreach (var button in mentalSkillButtonList)
                button.gameObject.SetActive(false);
        else
            foreach (var button in mentalSkillButtonList)
                button.gameObject.SetActive(true);
    }
}
