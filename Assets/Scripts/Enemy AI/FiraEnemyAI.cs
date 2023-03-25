using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiraEnemyAI : EnemyAI
{
    private System.Random random = new();

    private void Start() => enemyID = 1;

    public override List<string> CombatAI(out int soundID)
    {
        soundID = -1;
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        if (currentEnemyUnit.currentHP <= (int)(0.33 * currentEnemyUnit.maxHP))
            return BerserkMode();
        List<string> messageList = new();
        int attackProbability = random.Next(1, 101);
        int whoProbability = random.Next(1, CombatSystem.instance.allyUnits.Count + 1);
        if (attackProbability > 75 && currentEnemyUnit.currentMP >= 3)
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
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                messageList.Add(CombatSystem.instance.ReflectAction(currentEnemyUnit, -1, -CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, currentEnemyUnit), out _));
                CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
            }
            else
            {
                int totalDamage;
                Unit target;
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
                if (target == CombatSystem.instance.playerUnit)
                    CombatSystem.instance.playerIsHurting = true;
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " физического урона");
            }
        }
        return messageList;
    }

    private List<string> BerserkMode()
    {
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        List<string> messageList = new()
        {
            currentEnemyUnit.unitName + " в ярости и атакует бездумно"
        };
        int fireOrBase = random.Next(1, 3);
        int whoProbability = random.Next(1, CombatSystem.instance.allyUnits.Count + 1);
        if (fireOrBase != 1 && currentEnemyUnit.currentMP >= 3)
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
                    totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(3, true, currentEnemyUnit, target));
                }
                else
                {
                    target = CombatSystem.instance.allyUnits[whoProbability - 1];
                    totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(3, true, currentEnemyUnit, target));
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
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                messageList.Add(CombatSystem.instance.ReflectAction(currentEnemyUnit, -1, (int)(-1.15 * CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, currentEnemyUnit)), out _));
                CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
            }
            else
            {
                int totalDamage;
                Unit target;
                if (whoProbability == CombatSystem.instance.allyUnits.Count)
                {
                    target = CombatSystem.instance.playerUnit;
                    totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(0, true, currentEnemyUnit, target));
                }
                else
                {
                    target = CombatSystem.instance.allyUnits[whoProbability - 1];
                    totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(0, true, currentEnemyUnit, target));
                }

                target.TakeDamage(totalDamage);
                if (target == CombatSystem.instance.playerUnit)
                    CombatSystem.instance.playerIsHurting = true;
                messageList.Add(target.ApplyEffect(2));
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " физического урона");
            }
        }
        return messageList;
    }
}
