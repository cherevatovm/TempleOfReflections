using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyEnemyAI : EnemyAI
{
    private System.Random random = new();
    public List<int> trueSkills;
    public int countCopyTurns;

    public override List<string> CombatAI(out int soundID)
    {
        List<string> messageList = new();
        soundID = -1;
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        if (countCopyTurns == 1)
            messageList.Add(CopySkills());
        if (countCopyTurns >= 3)
        {
            countCopyTurns = 0;
            trueSkills.Clear();
        }

        CombatSystem.instance.curAllyID = random.Next(0, CombatSystem.instance.allyUnits.Count);
        Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
        //int randomSkillID = random.Next(0, currentEnemyUnit.availableMentalSkills.Length);
        //while(!currentEnemyUnit.availableMentalSkills[randomSkillID])
        //    randomSkillID = random.Next(0, currentEnemyUnit.availableMentalSkills.Length);
        int randomSkillID = random.Next(0, trueSkills.Count);

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
                0 => " ����������� �����",
                1 => " ������������� �����",
                2 => " �������������� �����",
                3 => " ��������� �����",
                _ => string.Empty,
            };
            messageList.Add(currentEnemyUnit.unitName + " ������� " + totalDamage + skillName);
        }
        currentEnemyUnit.ReduceCurrentMP(3);
        currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
        return messageList;
    }

    private string CopySkills()
    {
        CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].availableMentalSkills.CopyTo(CombatSystem.instance.
                enemyUnits[CombatSystem.instance.curEnemyID].availableMentalSkills, 0);

        for (int i = 0; i < CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID].availableMentalSkills.Length; i++)
        {
            if (CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID].availableMentalSkills[i])
                trueSkills.Add(i);
        }
        return CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID].unitName + " �������� ������ � " +
            CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID].unitName;
    }
}