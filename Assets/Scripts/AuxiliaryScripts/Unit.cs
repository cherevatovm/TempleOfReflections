using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Unit : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float restartDelay = 3f;
    private System.Random random = new();

    [HideInInspector] public int initMaxHP;
    [HideInInspector] public int initMaxMP;
    [HideInInspector] public int initMeleeAttackStrength;
    [HideInInspector] public int initMentalAttackStrength;
    [HideInInspector] public float[] initElementAffinities;

    public string unitName;    
    public int meleeAttackStrength;
    public int mentalAttackStrength;
    public float armorModifier = 1;
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;

    public int itemEffectTurnsCount = 0;
    public bool underItemEffect;
    public ItemWithEffect affectingItem;

    [HideInInspector] public bool isKnockedDown;
    [HideInInspector] public int knockedTurnsCount;
    [HideInInspector] public int knockedDownTimeout;
    [HideInInspector] public bool[] appliedEffect = new bool[3];
    [HideInInspector] public int underEffectTurnsCount = 0;

    public bool[] weaknesses;
    public bool[] resistances;
    public bool[] nulls;
    public float[] elementAffinities;

    private void Start()
    {
        if (CompareTag("Player"))
        {
            GameUI.instance.SetUI(this);
            initMaxHP = maxHP;
            initMaxMP = maxMP;
            initMeleeAttackStrength = meleeAttackStrength;
            initMentalAttackStrength = mentalAttackStrength;
            initElementAffinities = new float[4];
            System.Array.Copy(elementAffinities, initElementAffinities, 4);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (CompareTag("Player"))
            GameUI.instance.ChangeHP(currentHP);
    }

    public void Heal(int HP)
    {
        if (maxHP - currentHP > HP)
            currentHP += HP;
        else
            currentHP = maxHP;
        if (CompareTag("Player"))
            GameUI.instance.ChangeHP(currentHP);
    }

    public void ReduceCurrentMP(int MPcost)
    {
        currentMP -= MPcost;
        if (currentMP < 0)
            currentMP = 0;
        if (CompareTag("Player"))
            GameUI.instance.ChangeMP(currentMP);
    }

    public void IncreaseCurrentMP(int MP)
    {
        if (maxMP - currentMP > MP)
            currentMP += MP;
        else
            currentMP = maxMP;
        if (CompareTag("Player"))
            GameUI.instance.ChangeMP(currentMP);
    }

    public bool IsDead() => currentHP <= 0;

    public bool DoesHaveMP() => currentMP > 0;

    public void CopyStats(Unit otherUnit)
    {
        meleeAttackStrength = otherUnit.meleeAttackStrength;
        mentalAttackStrength = otherUnit.mentalAttackStrength;
        armorModifier = otherUnit.armorModifier;
        currentHP = otherUnit.currentHP;
        maxHP = otherUnit.maxHP;
        currentMP = otherUnit.currentMP;
        maxMP = otherUnit.maxMP;
        System.Array.Copy(otherUnit.weaknesses, weaknesses, 4);
        System.Array.Copy(otherUnit.resistances, resistances, 4);
        System.Array.Copy(otherUnit.nulls, nulls, 4);
        System.Array.Copy(otherUnit.elementAffinities, elementAffinities, 4);
    }

    public bool UnitEffectUpdate()
    {
        bool res = appliedEffect.Contains(true);
        if (res)
        {
            int index = System.Array.FindIndex(appliedEffect, elem => elem.Equals(true));
            switch (index)
            {
                case 0:
                    PsionaEffectUpdate();
                    break;
                case 1:
                    ElectraEffectUpdate();
                    break;
                case 2:
                    FiraEffectUpdate();
                    break;
            }
        }
        return res;
    }

    public string ApplyEffect(int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= appliedEffect.Length)
            return string.Empty;
        string message = string.Empty;
        if (!appliedEffect.Contains(true) && !resistances[effectIndex + 1] && !nulls[effectIndex + 1] && random.NextDouble() < CombatSystem.effectProbability)
        {
            appliedEffect[effectIndex] = true;
            message = effectIndex switch
            {
                0 => "�� " + unitName + " ������� ������ ���������� MP",
                1 => unitName + " ����������� ������ ��������������, ������� �� ��������� ���������",
                2 => unitName + " � ����!",
                _ => string.Empty,
            };
        }
        return message;
    }

    private void PsionaEffectUpdate()
    {
        if (appliedEffect[0] && underEffectTurnsCount < 3)
        {
            underEffectTurnsCount++;
            int takenMP = (int)(0.08 * maxMP);
            ReduceCurrentMP(takenMP);
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " ������ " + takenMP + " ����� MP";
        }
        else if (underEffectTurnsCount == 3)
        {
            appliedEffect[0] = false;
            underEffectTurnsCount = 0;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " ����������� �� �������";
        }
    }

    private void ElectraEffectUpdate()
    {
        if (appliedEffect[1] && underEffectTurnsCount < 2)
        {
            underEffectTurnsCount++;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " �������� ���������, ����� ���������";
        }
        else if (underEffectTurnsCount == 2)
        {
            appliedEffect[1] = false;
            underEffectTurnsCount = 0;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " ����������� �� �������";
        }
    }

    private void FiraEffectUpdate()
    {
        if (appliedEffect[2] && underEffectTurnsCount < 3)
        {
            underEffectTurnsCount++;
            int takenHP = (int)(0.05 * maxHP);
            TakeDamage(takenHP);
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " ������ " + takenHP + " ����� ��������";
        }
        else if (underEffectTurnsCount == 3)
        {
            appliedEffect[2] = false;
            underEffectTurnsCount = 0;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " ����������� �� �������";
        }
    }

    public void Death()
    {
        SoundManager.PlaySound(SoundManager.Sound.EnterCombat);
        StopAllCoroutines();
        rb.bodyType = RigidbodyType2D.Static;
        Inventory.instance.Close();
        GameUI.instance.ShowDeathscreen();
        Invoke(nameof(Restart), restartDelay);
    }

    private void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
