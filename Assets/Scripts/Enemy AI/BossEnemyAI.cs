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
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        int whoProbability = random.Next(1, CombatSystem.instance.allyUnits.Count + 1);
        if (currentEnemyUnit.currentHP <= (int)(0.15 * currentEnemyUnit.maxHP) && currentEnemyUnit.currentMP >= 12)
        {
            currentEnemyUnit.Heal((int)(0.1 * currentEnemyUnit.maxHP));
            currentEnemyUnit.ReduceCurrentMP(12);
            currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
            currentEnemyUnit.combatHUD.ChangeHP(currentEnemyUnit.currentHP);
            messageList.Add(currentEnemyUnit.unitName + " пополнил себе" + (int)(0.1 * currentEnemyUnit.maxHP) + " единиц здоровья");
            return messageList;
        }
        int attackProbability = random.Next(1, 101);
        if (attackProbability <= 40 && currentEnemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.PsiSkill);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                string message = CombatSystem.instance.ReflectAction(currentEnemyUnit, 0, -CombatSystem.instance.CalcAffinityDamage(1, true, currentEnemyUnit, currentEnemyUnit), out string effectMessage);
                messageList.Add(effectMessage);
                messageList.Add(message);
                CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
            }
            else
            {
                int totalDamage;
                Unit target;
                if (whoProbability == CombatSystem.instance.allyUnits.Count)
                {
                    target = CombatSystem.instance.playerUnit;
                    totalDamage = CombatSystem.instance.CalcAffinityDamage(1, true, currentEnemyUnit, target);
                }
                else
                {
                    target = CombatSystem.instance.allyUnits[whoProbability - 1];
                    totalDamage = CombatSystem.instance.CalcAffinityDamage(1, true, currentEnemyUnit, target);
                }

                target.TakeDamage(totalDamage);
                if (target == CombatSystem.instance.playerUnit)
                    CombatSystem.instance.playerIsHurting = true;
                messageList.Add(target.ApplyEffect(0));
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " псионического урона");
            }
            currentEnemyUnit.ReduceCurrentMP(3);
            currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
        }
        else if (attackProbability >= 41 && attackProbability <= 85 && currentEnemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.FiraSkill);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                string message = CombatSystem.instance.ReflectAction(currentEnemyUnit, 2, -CombatSystem.instance.CalcAffinityDamage(3, true, currentEnemyUnit, currentEnemyUnit), out string effectMessage);
                messageList.Add(effectMessage);
                messageList.Add(message);
                CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
            }
            else
            {
                int totalDamage;
                Unit target;
                if (whoProbability == CombatSystem.instance.allyUnits.Count)
                {
                    target = CombatSystem.instance.playerUnit;
                    totalDamage = CombatSystem.instance.CalcAffinityDamage(3, true, currentEnemyUnit, target);
                }
                else
                {
                    target = CombatSystem.instance.allyUnits[whoProbability - 1];
                    totalDamage = CombatSystem.instance.CalcAffinityDamage(3, true, currentEnemyUnit, target);
                }

                target.TakeDamage(totalDamage);
                if (target == CombatSystem.instance.playerUnit)
                    CombatSystem.instance.playerIsHurting = true;
                messageList.Add(target.ApplyEffect(2));
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " огненного урона");
            }
            currentEnemyUnit.ReduceCurrentMP(3);
            currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
        }
        else
        {
            int totalDamage;
            Unit target;
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            if (whoProbability == CombatSystem.instance.allyUnits.Count)
            {
                target = CombatSystem.instance.playerUnit;
                totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, target);
            }
            else
            {
                target = CombatSystem.instance.allyUnits[whoProbability - 1];
                totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, target);
            }
            target.TakeDamage(totalDamage);
            messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " физического урона и применяет вампиризм");
            currentEnemyUnit.Heal((int)(0.2 * totalDamage));
            currentEnemyUnit.combatHUD.ChangeHP(currentEnemyUnit.currentHP);
        }
        return messageList;
    }
}
       

