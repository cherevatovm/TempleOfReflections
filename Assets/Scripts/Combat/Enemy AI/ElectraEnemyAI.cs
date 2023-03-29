using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectraEnemyAI : EnemyAI
{
    private System.Random random = new();

    public override List<string> CombatAI(out int soundID)
    {
        List<string> messageList = new();
        soundID = -1;
        int attackProbability = random.Next(1, 101);
        CombatSystem.instance.curAllyID = random.Next(0, CombatSystem.instance.allyUnits.Count);
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        if (attackProbability > 60 && currentEnemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                string message = CombatSystem.instance.ReflectAction(currentEnemyUnit, 1, -CombatSystem.instance.CalcAffinityDamage(2, true, currentEnemyUnit, currentEnemyUnit), out string effectMessage);
                messageList.Add(effectMessage);
                messageList.Add(message);
                currentEnemyUnit.ReduceCurrentMP(3);
                currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
                CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
                EnemyInfoPanel.instance.ChangeKnownAffinities(currentEnemyUnit.enemyID, 2);
                return messageList;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(2, true, currentEnemyUnit, target);
                target.TakeDamage(totalDamage);
                CombatSystem.instance.allyCombatControllers[CombatSystem.instance.curAllyID].isHurting = true;
                messageList.Add(target.ApplyEffect(1));              
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " электрического урона");
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
                EnemyInfoPanel.instance.ChangeKnownAffinities(currentEnemyUnit.enemyID, 0);
                return messageList;
            }
            else
            {               
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, target);
                target.TakeDamage(totalDamage);
                CombatSystem.instance.allyCombatControllers[CombatSystem.instance.curAllyID].isHurting = true;
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " физического урона");
            }
        }
        if (currentEnemyUnit.currentHP >= (int)(0.7 * currentEnemyUnit.maxHP))
        {
            soundID = 1;
            int totalDamage = (int)(0.5 * CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, target));
            target.TakeDamage(totalDamage);
            messageList.Add(currentEnemyUnit.unitName + " использует быструю атаку и наносит " + totalDamage + " физического урона");
        }
        return messageList;
    }
}
