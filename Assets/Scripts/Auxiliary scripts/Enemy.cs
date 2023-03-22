using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public GameObject[] enemyPrefabsForCombat;
    [HideInInspector] public CombatHUD combatHUD;
    public int coinsDropped;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !CombatSystem.instance.isInCombat)
        {
            CombatSystem.instance.encounteredEnemy = this;
            StartCoroutine(CombatSystem.instance.SetupBattle());
        }
        else if (collision.CompareTag("Player"))
            transform.position = GetComponent<EnemyMovement>().GetFirstWaypointsTransform().position;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CombatSystem.instance.isInCombat && CombatSystem.instance.isChoosingEnemyForAttack)
            {
                CombatSystem.instance.curEnemyID = CombatSystem.instance.enemyUnits.IndexOf(this);
                CombatSystem.instance.isChoosingEnemyForAttack = false;
                CombatSystem.instance.combatUI.combatDialogue.text = CombatSystem.instance.playerUnit.unitName + " начинает атаку";
                StartCoroutine(CombatSystem.instance.PlayerAttack(CombatSystem.instance.damageTypeID, CombatSystem.instance.isMental));
            }
            else if (CombatSystem.instance.isInCombat && CombatSystem.instance.isChoosingEnemyForItem)
            {
                CombatSystem.instance.curEnemyID = CombatSystem.instance.enemyUnits.IndexOf(this);
                CombatSystem.instance.isChoosingEnemyForItem = false;
                CombatSystem.instance.activeSlot.UseItemInSlot();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //EnemyInfoPanel.instance.gameObject.SetActive(true);
        }
    }

    private void OnMouseDown()
    {
        if (CombatSystem.instance.isInCombat && CombatSystem.instance.isChoosingEnemyForAttack)
        {
            CombatSystem.instance.curEnemyID = CombatSystem.instance.enemyUnits.IndexOf(this);
            CombatSystem.instance.isChoosingEnemyForAttack = false;
            CombatSystem.instance.combatUI.combatDialogue.text = CombatSystem.instance.playerUnit.unitName + " начинает атаку";
            StartCoroutine(CombatSystem.instance.PlayerAttack(CombatSystem.instance.damageTypeID, CombatSystem.instance.isMental));
        }
        else if (CombatSystem.instance.isInCombat && CombatSystem.instance.isChoosingEnemyForItem)
        {
            CombatSystem.instance.curEnemyID = CombatSystem.instance.enemyUnits.IndexOf(this);
            CombatSystem.instance.isChoosingEnemyForItem = false;
            CombatSystem.instance.activeSlot.UseItemInSlot();
        }
    }
}
