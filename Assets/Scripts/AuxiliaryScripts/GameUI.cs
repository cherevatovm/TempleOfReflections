using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TMP_Text hpCounter;
    [SerializeField] TMP_Text mpCounter;
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider mpSlider;
    [SerializeField] GameObject exitMenu;
    [SerializeField] GameObject noteMenu;
    public static GameUI instance;

    void Awake() => instance = this;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowOrHideExitMenu();
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

    public void ShowOrHideExitMenu()
    {
        if (exitMenu.activeSelf)
            exitMenu.SetActive(false);
        else
            exitMenu.SetActive(true);
    }

    public void ShowOrHideNoteMenu()
    {
        if (noteMenu.activeSelf)
            noteMenu.SetActive(false);
        else
            noteMenu.SetActive(true);
    }

    public void ShowDeathscreen() => transform.GetChild(0).gameObject.SetActive(true);

    public void ExitGame() => Application.Quit();
}
