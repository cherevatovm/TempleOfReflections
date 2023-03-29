using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Unit
{
    [SerializeField] private float restartDelay = 3f;
    [HideInInspector] public int initMaxHP;
    [HideInInspector] public int initMaxMP;
    [HideInInspector] public int initMeleeAttackStrength;
    [HideInInspector] public int initMentalAttackStrength;
    [HideInInspector] public float[] initElementAffinities;

    private void Start()
    {
        GameUI.instance.SetUI(this);
        initMaxHP = maxHP;
        initMaxMP = maxMP;
        initMeleeAttackStrength = meleeAttackStrength;
        initMentalAttackStrength = mentalAttackStrength;
        initElementAffinities = new float[4];
        System.Array.Copy(elementAffinities, initElementAffinities, 4);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        GameUI.instance.ChangeHP(currentHP);
    }

    public override void Heal(int HP)
    {
        base.Heal(HP);
        GameUI.instance.ChangeHP(currentHP);
    }

    public override void ReduceCurrentMP(int MPcost)
    {
        base.ReduceCurrentMP(MPcost);
        GameUI.instance.ChangeMP(currentMP);
    }

    public override void IncreaseCurrentMP(int MP)
    {
        base.IncreaseCurrentMP(MP);
        GameUI.instance.ChangeMP(currentMP);
    }

    public void CopyStats(Player otherUnit)
    {
        currentHP = otherUnit.currentHP;
        currentMP = otherUnit.currentMP;
        if (!CombatSystem.instance.isInCombat)
        {
            maxHP = otherUnit.maxHP;
            maxMP = otherUnit.maxMP;
            armorModifier = otherUnit.armorModifier;
            meleeAttackStrength = otherUnit.meleeAttackStrength;
            mentalAttackStrength = otherUnit.mentalAttackStrength;            
            System.Array.Copy(otherUnit.weaknesses, weaknesses, 4);
            System.Array.Copy(otherUnit.resistances, resistances, 4);
            System.Array.Copy(otherUnit.nulls, nulls, 4);
            System.Array.Copy(otherUnit.elementAffinities, elementAffinities, 4);
            System.Array.Copy(otherUnit.availableMentalSkills, availableMentalSkills, 4);
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
