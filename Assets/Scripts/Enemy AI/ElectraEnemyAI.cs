using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectraEnemyAI : EnemyAI
{
    private System.Random random = new();

    private void Start() => enemyID = 0;

    public override List<string> CombatAI(out int soundID)
    {
        List<string> messageList = new();
        soundID = -1;
        int attackProbability = random.Next(1, 101);
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
                return messageList;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(2, true, currentEnemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(CombatSystem.instance.playerUnit.ApplyEffect(1));              
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
                return messageList;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " физического урона");
            }
        }
        if (currentEnemyUnit.currentHP >= (int)(0.7 * currentEnemyUnit.maxHP))
        {
            soundID = 1;
            int totalDamage = (int)(0.5 * CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, CombatSystem.instance.playerUnit));
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            messageList.Add(currentEnemyUnit.unitName + " использует быструю атаку и наносит " + totalDamage + " физического урона");
        }
        return messageList;
    }
}
