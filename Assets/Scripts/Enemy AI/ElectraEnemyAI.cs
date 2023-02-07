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
        if (attackProbability > 60 && CombatSystem.instance.enemyUnit.currentMP >= 3)
        {
            SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                string message = CombatSystem.instance.ReflectAction(CombatSystem.instance.enemyUnit, 1, -CombatSystem.instance.CalcAffinityDamage(2, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.enemyUnit), out string effectMessage);
                messageList.Add(effectMessage);
                messageList.Add(message);
                CombatSystem.instance.enemyIsHurting = true;
                CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
                CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
                return messageList;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(2, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(CombatSystem.instance.playerUnit.ApplyEffect(1));              
                messageList.Add("Враг наносит " + totalDamage + " электрического урона");
            }           
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                messageList.Add(CombatSystem.instance.ReflectAction(CombatSystem.instance.enemyUnit, -1, -CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.enemyUnit), out _));
                CombatSystem.instance.enemyIsHurting = true;
                return messageList;
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add("Враг наносит " + totalDamage + " физического урона");
            }
        }
        if (CombatSystem.instance.enemyUnit.currentHP >= (int)(0.7 * CombatSystem.instance.enemyUnit.maxHP))
        {
            soundID = 1;
            int totalDamage = (int)(0.5 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            messageList.Add("Враг использует быструю атаку и наносит " + totalDamage + " физического урона");
        }
        return messageList;
    }
}
