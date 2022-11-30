using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemyAI : EnemyAI
{
    System.Random random = new System.Random();

    public override void CombatAI(out string effectMessage)
    {
        effectMessage = string.Empty;
        if (CombatSystem.instance.enemyUnit.currentHP <= (int)(0.33 * CombatSystem.instance.enemyUnit.maxHP))
            StartCoroutine(BerserkMode());
        else
        {
            int attackProbability = random.Next(1, 101);
            if (attackProbability <= 75)
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.combatUI.combatDialogue.text = "¬раг наносит " + totalDamage + " физического урона";
            }
            else if (attackProbability > 75)
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                effectMessage = "¬раг наносит " + totalDamage + " огненного урона";
                CombatSystem.instance.playerUnit.FiraEffect();
            }
        }
    }

    IEnumerator BerserkMode()
    {
        yield return new WaitForSeconds(1f);
        CombatSystem.instance.combatUI.combatDialogue.text = "¬раг в €рости и атакует бездумно";
        yield return new WaitForSeconds(1f);
        int fireOrBase = random.Next(1, 3);
        if (fireOrBase == 1)
        {
            int totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.combatUI.combatDialogue.text = "¬раг наносит " + totalDamage + " физического урона";
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            int totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.playerUnit.FiraEffect();
            yield return new WaitForSeconds(1.5f);
            CombatSystem.instance.combatUI.combatDialogue.text = "¬раг наносит " + totalDamage + " огненного урона";
        }

    }
}
