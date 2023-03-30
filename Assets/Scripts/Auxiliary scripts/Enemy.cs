using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public string enemyDescription;
    public int enemyID;
    public int coinsDropped;
    public GameObject[] enemyPrefabsForCombat;
    
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
        if (!CombatSystem.instance.isInCombat || EnemyInfoPanel.instance.isActiveAndEnabled)
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
        else if (Input.GetMouseButtonDown(1))
        {
            if (!Inventory.instance.isOpen && CombatSystem.instance.combatState == CombatState.ALLY_TURN)
                EnemyInfoPanel.instance.OpenEnemyInfoPanel(this);
        }
    }
}
