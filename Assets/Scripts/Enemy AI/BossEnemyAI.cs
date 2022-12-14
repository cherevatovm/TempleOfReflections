using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAI : EnemyAI
{
    System.Random random = new System.Random();

    void Start() => enemyID = 2;

    public override void CombatAI(out string effectMessage)
    {
        effectMessage = string.Empty;
        if (CombatSystem.instance.enemyUnit.currentHP <= (int)(0.15 * CombatSystem.instance.enemyUnit.maxHP) && CombatSystem.instance.enemyUnit.currentMP >= 12)
        {
            CombatSystem.instance.enemyUnit.Heal((int)(0.1 * CombatSystem.instance.enemyUnit.maxHP));
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(12);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
            CombatSystem.instance.enemyHUD.ChangeHP(CombatSystem.instance.enemyUnit.currentHP);
            CombatSystem.instance.combatUI.combatDialogue.text = "???? ???????? ????" + (int)(0.1 * CombatSystem.instance.enemyUnit.maxHP) + " ?????? ????????";
            return;
        }
        int attackProbability = random.Next(1, 101);
        if (attackProbability <= 40 && CombatSystem.instance.enemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.PsiSkill);
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(1, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
            effectMessage = "???? ??????? " + totalDamage + " ????????????? ?????";
            CombatSystem.instance.playerUnit.PsionaEffect();
        }
        else if (attackProbability >= 41 && attackProbability <= 85 && CombatSystem.instance.enemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.FiraSkill);
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
            effectMessage = "???? ??????? " + totalDamage + " ????????? ?????";
            CombatSystem.instance.playerUnit.FiraEffect();
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.combatUI.combatDialogue.text = "???? ??????? " + totalDamage + " ??????????? ????? ? ????????? ?????????";
            CombatSystem.instance.enemyUnit.Heal((int)(0.2 * totalDamage));
            CombatSystem.instance.enemyHUD.ChangeHP(CombatSystem.instance.enemyUnit.currentHP);
        }
    }
}
       

