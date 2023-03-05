using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public GameObject[] enemyPrefabsForCombat;
    [HideInInspector] public CombatHUD combatHUD;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CombatSystem.instance.encounteredEnemy = this;
            StartCoroutine(CombatSystem.instance.SetupBattle());
        }
    }

    private void OnMouseDown()
    {
        if (CombatSystem.instance.isInCombat && CombatSystem.instance.isChoosingEnemy)
        {
            CombatSystem.instance.curEnemyID = CombatSystem.instance.enemyUnits.IndexOf(this);
            CombatSystem.instance.isChoosingEnemy = false;
            CombatSystem.instance.combatUI.combatDialogue.text = CombatSystem.instance.playerUnit.unitName + " начинает атаку";
            StartCoroutine(CombatSystem.instance.PlayerAttack(CombatSystem.instance.damageTypeID, CombatSystem.instance.isMental));
        }
    }
}
