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
    [SerializeField] public GameObject exitUI;
    [SerializeField] GameObject noteUI;
    [SerializeField] GameObject dialogueUI;
    public static GameUI instance;

    void Awake() => instance = this;

    void Update()
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
            Inventory.instance.Close();
            return;
        }
        else if (noteUI.activeSelf)
        {
            NoteManager.instance.EndReading();
            return;
        }
        else if (dialogueUI.activeSelf)
            return;
        if (exitUI.activeSelf)
            exitUI.SetActive(false);
        else
            exitUI.SetActive(true);
    }

    public void ShowOrHideNoteUI() //На данный момент бесполезно
    {
        if (noteUI.activeSelf)
            noteUI.SetActive(false);
        else
            noteUI.SetActive(true);
    }

    public bool IsSubmenuActive() => exitUI.activeSelf || noteUI.activeSelf || dialogueUI.activeSelf;

    public void ShowDeathscreen() => transform.GetChild(0).gameObject.SetActive(true);

    public void ExitGame() => Application.Quit();
}
