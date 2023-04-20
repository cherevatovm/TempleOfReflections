using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyEnemyAI : EnemyAI
{
    private System.Random random = new();
    public override List<string> CombatAI(out int soundID)
    {
        List<string> messageList = new();
        soundID = -1;
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        if (currentEnemyUnit.countCopyTurns <= 2)
            CopySkills(currentEnemyUnit);
        else
            currentEnemyUnit.countCopyTurns = 0;

        CombatSystem.instance.curAllyID = random.Next(0, CombatSystem.instance.allyUnits.Count);
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        int randomSkillID = random.Next(0, currentEnemyUnit.availableMentalSkills.Length);
        while(!currentEnemyUnit.availableMentalSkills[randomSkillID])
            randomSkillID = random.Next(0, currentEnemyUnit.availableMentalSkills.Length);

        SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
        if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
        {
            string message = CombatSystem.instance.ReflectAction(currentEnemyUnit, randomSkillID - 1,
                -CombatSystem.instance.CalcAffinityDamage(randomSkillID, true, currentEnemyUnit, currentEnemyUnit), out string effectMessage);
            messageList.Add(effectMessage);
            messageList.Add(message);
            currentEnemyUnit.ReduceCurrentMP(3);
            currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
            CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
            EnemyInfoPanel.instance.ChangeKnownAffinities(currentEnemyUnit.enemyID, randomSkillID);
            return messageList;
        }
        else
        {
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(randomSkillID, true, currentEnemyUnit, target);
            target.TakeDamage(totalDamage);
            CombatSystem.instance.allyCombatControllers[CombatSystem.instance.curAllyID].isHurting = true;
            messageList.Add(target.ApplyEffect(randomSkillID - 1));
            string skillName;
            skillName = randomSkillID switch
            {
                0 => " физического урона",
                1 => " псионического урона",
                2 => " электрического урона",
                3 => " огненного урона",
                _ => string.Empty,
            };
            messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + skillName);
        }
        currentEnemyUnit.ReduceCurrentMP(3);
        currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);

        //while (!currentEnemyUnit.availableMentalSkills[randomSkillID])
        //{
        //    randomSkillID = random.Next(0, currentEnemyUnit.availableMentalSkills.Length);
        //    if (currentEnemyUnit.availableMentalSkills[randomSkillID])
        //    {
        //        SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
        //        if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
        //        {
        //            string message = CombatSystem.instance.ReflectAction(currentEnemyUnit, randomSkillID - 1, 
        //                -CombatSystem.instance.CalcAffinityDamage(randomSkillID, true, currentEnemyUnit, currentEnemyUnit), out string effectMessage);
        //            messageList.Add(effectMessage);
        //            messageList.Add(message);
        //            currentEnemyUnit.ReduceCurrentMP(3);
        //            currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
        //            CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
        //            EnemyInfoPanel.instance.ChangeKnownAffinities(currentEnemyUnit.enemyID, randomSkillID);
        //            return messageList;
        //        }
        //        else
        //        {
        //            int totalDamage = CombatSystem.instance.CalcAffinityDamage(randomSkillID, true, currentEnemyUnit, target);
        //            target.TakeDamage(totalDamage);
        //            CombatSystem.instance.allyCombatControllers[CombatSystem.instance.curAllyID].isHurting = true;
        //            messageList.Add(target.ApplyEffect(randomSkillID - 1));
        //            string skillName;
        //            skillName = randomSkillID switch
        //            {
        //                0 => " физического урона",
        //                1 => " псионического урона",
        //                2 => " электрического урона",
        //                3 => " огненного урона",
        //                _ => string.Empty,
        //            };
        //            messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + skillName);
        //        }
        //        currentEnemyUnit.ReduceCurrentMP(3);
        //        currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
        //    }
        //}
        return messageList;
    }

    private void CopySkills(Enemy enemy)
    {
        for(int i = 0; i < CombatSystem.instance.allyUnits.Count; i++)
        {
            if (i == CombatSystem.instance.allyUnits.Count - 1)
                enemy.alreadyCopied.Clear();
            if (!enemy.alreadyCopied.Contains(i) && enemy.countCopyTurns == 0)
            {
                enemy.alreadyCopied.Add(i);
                for (int j = 0; j < CombatSystem.instance.allyUnits[i].availableMentalSkills.Length; j++)
                    enemy.availableMentalSkills[j] = CombatSystem.instance.allyUnits[i].availableMentalSkills[j];
            } 
        }
    }
}
