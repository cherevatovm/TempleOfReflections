using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAI : EnemyAI
{
    System.Random random = new System.Random();

    public override List<string> CombatAI(out int soundID)
    {
        List<string> messageList = new();
        soundID = -1;
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        if (currentEnemyUnit.currentHP <= (int)(0.15 * currentEnemyUnit.maxHP) && currentEnemyUnit.currentMP >= 12)
        {
            currentEnemyUnit.Heal((int)(0.1 * currentEnemyUnit.maxHP));
            currentEnemyUnit.ReduceCurrentMP(12);
            currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
            currentEnemyUnit.combatHUD.ChangeHP(currentEnemyUnit.currentHP);
            messageList.Add(currentEnemyUnit.unitName + " �������� ����" + (int)(0.1 * currentEnemyUnit.maxHP) + " ������ ��������");
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
                EnemyInfoPanel.instance.ChangeKnownAffinities(currentEnemyUnit.enemyID, 1);
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(1, true, currentEnemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(CombatSystem.instance.playerUnit.ApplyEffect(0));
                messageList.Add(currentEnemyUnit.unitName + " ������� " + totalDamage + " ������������� �����");
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
                EnemyInfoPanel.instance.ChangeKnownAffinities(currentEnemyUnit.enemyID, 3);
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(3, true, currentEnemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(CombatSystem.instance.playerUnit.ApplyEffect(2));
                messageList.Add(currentEnemyUnit.unitName + " ������� " + totalDamage + " ��������� �����");
            }
            currentEnemyUnit.ReduceCurrentMP(3);
            currentEnemyUnit.combatHUD.ChangeMP(currentEnemyUnit.currentMP);
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, CombatSystem.instance.playerUnit);
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            messageList.Add(currentEnemyUnit.unitName + " ������� " + totalDamage + " ����������� ����� � ��������� ���������");
            currentEnemyUnit.Heal((int)(0.2 * totalDamage));
            currentEnemyUnit.combatHUD.ChangeHP(currentEnemyUnit.currentHP);
        }
        return messageList;
    }
}
       

