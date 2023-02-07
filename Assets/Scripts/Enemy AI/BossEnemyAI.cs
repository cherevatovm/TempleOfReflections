using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAI : EnemyAI
{
    System.Random random = new System.Random();

    private void Start() => enemyID = 2;

    public override List<string> CombatAI(out int soundID)
    {
        List<string> messageList = new();
        soundID = -1;
        if (CombatSystem.instance.enemyUnit.currentHP <= (int)(0.15 * CombatSystem.instance.enemyUnit.maxHP) && CombatSystem.instance.enemyUnit.currentMP >= 12)
        {
            CombatSystem.instance.enemyUnit.Heal((int)(0.1 * CombatSystem.instance.enemyUnit.maxHP));
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(12);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
            CombatSystem.instance.enemyHUD.ChangeHP(CombatSystem.instance.enemyUnit.currentHP);
            messageList.Add("Враг пополнил себе" + (int)(0.1 * CombatSystem.instance.enemyUnit.maxHP) + " единиц здоровья");
            return messageList;
        }
        int attackProbability = random.Next(1, 101);
        if (attackProbability <= 40 && CombatSystem.instance.enemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.PsiSkill);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                string message = CombatSystem.instance.ReflectAction(CombatSystem.instance.enemyUnit, 0, -CombatSystem.instance.CalcAffinityDamage(1, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.enemyUnit), out string effectMessage);
                messageList.Add(effectMessage);
                messageList.Add(message);
                CombatSystem.instance.enemyIsHurting = true;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(1, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(CombatSystem.instance.playerUnit.ApplyEffect(0));
                messageList.Add("Враг наносит " + totalDamage + " псионического урона");
            }
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
        }
        else if (attackProbability >= 41 && attackProbability <= 85 && CombatSystem.instance.enemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.FiraSkill);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                string message = CombatSystem.instance.ReflectAction(CombatSystem.instance.enemyUnit, 2, -CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.enemyUnit), out string effectMessage);
                messageList.Add(effectMessage);
                messageList.Add(message);
                CombatSystem.instance.enemyIsHurting = true;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(CombatSystem.instance.playerUnit.ApplyEffect(2));
                messageList.Add("Враг наносит " + totalDamage + " огненного урона");
            }
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            messageList.Add("Враг наносит " + totalDamage + " физического урона и применяет вампиризм");
            CombatSystem.instance.enemyUnit.Heal((int)(0.2 * totalDamage));
            CombatSystem.instance.enemyHUD.ChangeHP(CombatSystem.instance.enemyUnit.currentHP);
        }
        return messageList;
    }
}
       

