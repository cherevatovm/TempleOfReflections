using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Unit : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    private System.Random random = new();

    public string unitName;
    public CombatHUD combatHUD;
    public int meleeAttackStrength;
    public int mentalAttackStrength;
    public float armorModifier = 1;
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;

    [HideInInspector] public bool isKnockedDown;
    [HideInInspector] public int knockedTurnsCount;
    [HideInInspector] public int knockedDownTimeout;
    [HideInInspector] public bool[] appliedEffect = new bool[3];
    [HideInInspector] public int underEffectTurnsCount = 0;
    [HideInInspector] public List<ItemWithEffect> affectingItems = new();

    public bool[] weaknesses = new bool[4];
    public bool[] resistances = new bool[4];
    public bool[] nulls = new bool[4];
    public float[] elementAffinities = new float[4];
    public bool[] availableMentalSkills = new bool[4];

    private void OnMouseOver()
    {
        if (!CombatSystem.instance.isInCombat || EnemyInfoPanel.instance.isActiveAndEnabled)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (CombatSystem.instance.isChoosingAllyForSkill)
            {
                CombatSystem.instance.tempAllyID = CombatSystem.instance.allyUnits.IndexOf(this);
                CombatSystem.instance.isChoosingAllyForSkill = false;
                CombatSystem.instance.combatUI.combatDialogue.text = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].unitName + " применяет целительный навык";
                StartCoroutine(CombatSystem.instance.AllyRegenaSkill());
            }
            else if (CombatSystem.instance.isChoosingAllyForItem)
            {
                CombatSystem.instance.tempAllyID = CombatSystem.instance.allyUnits.IndexOf(this);
                CombatSystem.instance.isChoosingAllyForItem = false;
                CombatSystem.instance.activeSlot.UseItemInSlot();
            }
        }
    }

    public virtual void TakeDamage(int damage) => currentHP -= damage;

    public virtual void Heal(int HP)
    {
        if (maxHP - currentHP > HP)
            currentHP += HP;
        else
            currentHP = maxHP;
    }

    public virtual void ReduceCurrentMP(int MPcost)
    {
        currentMP -= MPcost;
        if (currentMP < 0)
            currentMP = 0;
    }

    public virtual void IncreaseCurrentMP(int MP)
    {
        if (maxMP - currentMP > MP)
            currentMP += MP;
        else
            currentMP = maxMP;
    }

    public bool IsDead() => currentHP <= 0;

    public bool DoesHaveMP() => currentMP > 0;

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
        if (!appliedEffect.Contains(true) && !resistances[effectIndex + 1] && !nulls[effectIndex + 1] && random.NextDouble() < CombatSystem.instance.effectProbability && currentHP > 0)
        {
            appliedEffect[effectIndex] = true;
            message = effectIndex switch
            {
                0 => "На " + unitName + " наложен эффект поглощения MP",
                1 => unitName + " оковывает электричество, которое не позволяет ходить",
                2 => unitName + " в огне!",
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
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " теряет " + takenMP + " очков MP";
        }
        else if (underEffectTurnsCount == 3)
        {
            appliedEffect[0] = false;
            underEffectTurnsCount = 0;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " оправляется от эффекта";
        }
    }

    private void ElectraEffectUpdate()
    {
        if (appliedEffect[1] && underEffectTurnsCount < 2)
        {
            underEffectTurnsCount++;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " не может ходить из-за шока";
        }
        else if (underEffectTurnsCount == 2)
        {
            appliedEffect[1] = false;
            underEffectTurnsCount = 0;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " оправляется от эффекта";
        }
    }

    private void FiraEffectUpdate()
    {
        if (appliedEffect[2] && underEffectTurnsCount < 3)
        {
            underEffectTurnsCount++;
            int takenHP = (int)(0.05 * maxHP);
            TakeDamage(takenHP);
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " теряет " + takenHP + " очков здоровья";
        }
        else if (underEffectTurnsCount == 3)
        {
            appliedEffect[2] = false;
            underEffectTurnsCount = 0;
            CombatSystem.instance.combatUI.combatDialogue.text = unitName + " оправляется от эффекта";
        }
    }
}
