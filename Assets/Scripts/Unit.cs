using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Unit : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Canvas unitCanvas;
    [SerializeField] float restartDelay = 1.5f;
    System.Random random = new System.Random();

    public string unitName;    
    public int meleeAttackStrength;
    public int mentalAttackStrength;
    public float armorModifier = 1;
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    
    public bool isKnockedDown;
    public int knockedTurnsCount;
    public int knockedDownTimeout;
    public bool[] appliedEffect = new bool[3];
    public int underEffectTurnsCount = 0;

    public bool[] weaknesses;
    public bool[] resistances;
    public bool[] nulls;
    public float[] elementAffinities;

    public void TakeDamage(int damage) => currentHP -= damage;

    public void Heal(int HP)
    {
        if (maxHP - currentHP > HP)
            currentHP += HP;
        else
            currentHP = maxHP;
    }

    public void ReduceCurrentMP(int MPcost)
    {
        currentMP -= MPcost;
        if (currentMP < 0)
            currentMP = 0;
    }

    public void IncreaseCurrentMP(int MP)
    {
        if (maxMP - currentMP > MP)
            currentMP += MP;
        else
            currentMP = maxMP;
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
        System.Array.Copy(otherUnit.weaknesses, weaknesses, 4);
        System.Array.Copy(otherUnit.resistances, resistances, 4);
        System.Array.Copy(otherUnit.nulls, nulls, 4);
        System.Array.Copy(otherUnit.elementAffinities, elementAffinities, 4);
    }

    public void UnitEffectUpdate()
    {
        if (appliedEffect.Contains(true))
        {
            int index = System.Array.FindIndex(appliedEffect, elem => elem.Equals(true));
            switch (index)
            {
                case 0:
                    PsionaEffect();
                    break;
                case 1:
                    ElectraEffect();
                    break;
                case 2:
                    FiraEffect();
                    break;
            }
        }
    }

    public void PsionaEffect()
    {
        if (!appliedEffect.Contains(true) && !resistances[1] && !nulls[1] && random.NextDouble() < CombatSystem.effectProbability)
        {
            appliedEffect[0] = true;
            CombatSystem.instance.combatUI.combatDialogue.text = "�� " + unitName + " ������� ������ ���������� MP";
        }
        else if (appliedEffect[0] && underEffectTurnsCount < 3)
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

    public void ElectraEffect()
    {
        if (!appliedEffect.Contains(true) && !resistances[2] && !nulls[2] && random.NextDouble() < CombatSystem.effectProbability)
        {
            appliedEffect[1] = true;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " ����������� ������ ��������������, ������� �� ��������� ���������";
        }
        else if (appliedEffect[1] && underEffectTurnsCount < 2)
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

    public void FiraEffect()
    {
        if (!appliedEffect.Contains(true) && !resistances[3] && !nulls[3] && random.NextDouble() < CombatSystem.effectProbability)
        {
            appliedEffect[2] = true;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " � ����!";
        }
        else if (appliedEffect[2] && underEffectTurnsCount < 3)
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
        Debug.Log("Game over");
        rb.bodyType = RigidbodyType2D.Static;
        if (unitName == "Player")
            unitCanvas.gameObject.SetActive(true);
        Invoke(nameof(Restart), restartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
