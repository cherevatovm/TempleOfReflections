using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatHUD : MonoBehaviour
{
    TMP_Text textName;
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider mpSlider;

    public void SetHUD(Unit unit)
    {
        textName.text = unit.name;
        hpSlider.maxValue = unit.maxHP;
        mpSlider.maxValue = unit.maxMP;
        hpSlider.value = unit.currentHP;
        mpSlider.value = unit.currentMP;
    }

    public void ChangeHP(int hp) => hpSlider.value = hp;

    public void ChangeMP(int mp) => mpSlider.value = mp;
}
