using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectraEnemyAI : EnemyAI
{
    System.Random random = new System.Random();

    void Start() => enemyID = 0;

    public override void CombatAI(out string effectMessage)
    {
        effectMessage = string.Empty;
        int attackProbability = random.Next(1, 101);
        if (attackProbability > 60 && CombatSystem.instance.enemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(2, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
            effectMessage = "Враг наносит " + totalDamage + " электрического урона";
            CombatSystem.instance.playerUnit.ElectraEffect();
            if (CombatSystem.instance.enemyUnit.currentHP >= (int)(0.7 * CombatSystem.instance.enemyUnit.maxHP))
                StartCoroutine(FastAttack(effectMessage));
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.combatUI.combatDialogue.text = "Враг наносит " + totalDamage + " физического урона";
            if (CombatSystem.instance.enemyUnit.currentHP >= (int)(0.7 * CombatSystem.instance.enemyUnit.maxHP))
                StartCoroutine(FastAttack(effectMessage));
        }
    }

    IEnumerator FastAttack(string effectMessage)
    {
        yield return new WaitForSeconds(1.5f);
        if (!string.IsNullOrEmpty(effectMessage))
        {
            CombatSystem.instance.combatUI.combatDialogue.text = effectMessage;
            effectMessage = string.Empty;
            yield return new WaitForSeconds(1.5f);
        }
        SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
        int totalDamage = (int)(0.5 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
        CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
        CombatSystem.instance.combatUI.combatDialogue.text = "Враг использует быструю атаку и наносит " + totalDamage + " физического урона";
    }
}
