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

    public void TakeDamage(int damage) => currentHP -= damage;

    public void ReduceCurrentMP(int MPcost) => currentMP -= MPcost;

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

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
