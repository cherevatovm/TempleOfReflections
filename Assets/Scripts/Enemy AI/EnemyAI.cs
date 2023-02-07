using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    public int enemyID;

    public abstract List<string> CombatAI(out int soundID);

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CombatSystem.instance.encounteredEnemy = this;
            StartCoroutine(CombatSystem.instance.SetupBattle());
        }
    }
}
