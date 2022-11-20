using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Canvas unitCanvas;
    [SerializeField] float restartDelay = 1.5f;
    
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
    
    public bool[] weaknesses;
    public bool[] resistances;
    public bool[] nulls;
    public float[] damageTypeAffinities;

    public int effectTurnsCount;
    public const int efftectProbability = 8;
    public CombatSystem cb;

    public void TakeDamage(int damage) => currentHP -= damage;

    public void ReduceCurrentMP(int MPcost) => currentMP -= MPcost;

    public void IncreaseCurrentMP(int MPcost) => currentMP += MPcost;

    public bool IsDead() => currentHP <= 0;

    public bool DoesHaveMP() => currentMP > 0;

    public void Death()
    {
        Debug.Log("Game over");
        rb.bodyType = RigidbodyType2D.Static;
        if (unitName == "Player")
            unitCanvas.gameObject.SetActive(true);
        Invoke(nameof(Restart), restartDelay);
    }

    public void PsionaEffect(Unit attackingUnit, Unit defendingUnit)
    {
        effectTurnsCount = 0;

        if (Random.Range(1, 101) <= efftectProbability && !defendingUnit.resistances[1])
        {
            effectTurnsCount++;
            if (effectTurnsCount <= 3)
            {
                int takenMP = (int) (0.08 * defendingUnit.maxMP);
                defendingUnit.ReduceCurrentMP(takenMP);
                attackingUnit.IncreaseCurrentMP(takenMP);
            }
        }
    }

    public void ElectroEffect(Unit defendingUnit)
    {
        effectTurnsCount = 0;

        if (Random.Range(1, 101) <= efftectProbability && !defendingUnit.resistances[2])
        {
            effectTurnsCount++;
            if (effectTurnsCount <= 2 && defendingUnit == cb.playerUnit)
                defendingUnit.cb.EnemyTurn();
            else if (effectTurnsCount <= 2 && defendingUnit == cb.enemyUnit)
                defendingUnit.cb.PlayerTurn();
        }
    }

    public void FireEffect(Unit defendingUnit)
    {
        effectTurnsCount = 0;

        if (Random.Range(1, 101) <= efftectProbability && !defendingUnit.resistances[resistances.Length - 1])
        {
            effectTurnsCount++;
            if (effectTurnsCount <= 2)
            {
                int damage = (int) (0.05 * defendingUnit.maxHP);
                defendingUnit.TakeDamage(damage);
            }
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
