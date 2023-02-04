using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectraEnemyAI : EnemyAI
{
    private System.Random random = new();

    private void Start() => enemyID = 0;

    public override void CombatAI(out string effectMessage)
    {
        int attackProbability = random.Next(1, 101);
        if (attackProbability > 60 && CombatSystem.instance.enemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                effectMessage = CombatSystem.instance.ReflectAction(CombatSystem.instance.enemyUnit, 1, -CombatSystem.instance.CalcAffinityDamage(2, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.enemyUnit));
                CombatSystem.instance.enemyIsHurting = true;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(2, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                CombatSystem.instance.playerUnit.ElectraEffect();               
                effectMessage = "Враг наносит " + totalDamage + " электрического урона";
            }           
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);            
            if (CombatSystem.instance.enemyUnit.currentHP >= (int)(0.7 * CombatSystem.instance.enemyUnit.maxHP))
                StartCoroutine(FastAttack(effectMessage));
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                effectMessage = CombatSystem.instance.ReflectAction(CombatSystem.instance.enemyUnit, -1, -CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.enemyUnit));
                CombatSystem.instance.enemyIsHurting = true;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                effectMessage = "Враг наносит " + totalDamage + " физического урона";
            }
            CombatSystem.instance.combatUI.combatDialogue.text = effectMessage;
            if (CombatSystem.instance.enemyUnit.currentHP >= (int)(0.7 * CombatSystem.instance.enemyUnit.maxHP))
                StartCoroutine(FastAttack(effectMessage));
        }
    }

    private IEnumerator FastAttack(string effectMessage)
    {
        yield return new WaitForSeconds(1.5f);
        if (!string.IsNullOrEmpty(effectMessage))
        {
            CombatSystem.instance.combatUI.combatDialogue.text = effectMessage;
            yield return new WaitForSeconds(1.5f);
        }
        SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
        int totalDamage = (int)(0.5 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
        CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
        CombatSystem.instance.combatUI.combatDialogue.text = "Враг использует быструю атаку и наносит " + totalDamage + " физического урона";
    }
}
