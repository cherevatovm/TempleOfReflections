using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public GameObject[] enemyPrefabsForCombat;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CombatSystem.instance.encounteredEnemy = this;
            StartCoroutine(CombatSystem.instance.SetupBattle());
        }
    }

    private void OnMouseOver()
    {
        if (!CombatSystem.instance.isInCombat)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (CombatSystem.instance.isChoosingEnemyForAttack)
            {
                CombatSystem.instance.curEnemyID = CombatSystem.instance.enemyUnits.IndexOf(this);
                CombatSystem.instance.isChoosingEnemyForAttack = false;
                CombatSystem.instance.combatUI.combatDialogue.text = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].unitName + " начинает атаку";
                StartCoroutine(CombatSystem.instance.AllyAttack(CombatSystem.instance.damageTypeID, CombatSystem.instance.isMental));
            }
            else if (CombatSystem.instance.isChoosingEnemyForItem)
            {
                CombatSystem.instance.curEnemyID = CombatSystem.instance.enemyUnits.IndexOf(this);
                CombatSystem.instance.isChoosingEnemyForItem = false;
                CombatSystem.instance.activeSlot.UseItemInSlot();
            }
        }
    }
}
