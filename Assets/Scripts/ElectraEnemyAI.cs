using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectraEnemyAI : EnemyAI
{
    System.Random random = new System.Random();

    public override void CombatAI()
    {
        int attackProbability = random.Next(1, 101);

        if (attackProbability <= 60)
        {
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.combatUI.combatDialogue.text = "Враг наносит " + totalDamage + " физического урона";

            if (!CombatSystem.instance.enemyUnit.appliedEffect[1]) 
                StartCoroutine(FastAttack());
        }

        else if (attackProbability > 60)
        {
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(2, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.combatUI.combatDialogue.text = "Враг наносит " + totalDamage + " электрического урона";
            CombatSystem.instance.playerUnit.ElectraEffect();

            if (!CombatSystem.instance.enemyUnit.appliedEffect[1])
                StartCoroutine(FastAttack());
        }
    }

    IEnumerator FastAttack()
    {
        if (CombatSystem.instance.enemyUnit.currentHP >= (int)(0.7 * CombatSystem.instance.enemyUnit.maxHP))
        {
            yield return new WaitForSeconds(1.5f);
            int totalDamage = (int)(0.5 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.combatUI.combatDialogue.text = "Враг использует быструю атаку и наносит " + totalDamage + " физического урона";
            yield return new WaitForSeconds(1.5f);
        }
    }
}
