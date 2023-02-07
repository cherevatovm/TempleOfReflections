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
        if (CombatSystem.instance.enemyUnit.currentHP <= (int)(0.33 * CombatSystem.instance.enemyUnit.maxHP))
            return BerserkMode();
        List<string> messageList = new();
        int attackProbability = random.Next(1, 101);
        if (attackProbability > 75 && CombatSystem.instance.enemyUnit.currentMP >= 3)
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
                messageList.Add("¬раг наносит " + totalDamage + " огненного урона");
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
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add("¬раг наносит " + totalDamage + " физического урона");
            }
        }
        return messageList;
    }

    private List<string> BerserkMode()
    {
        List<string> messageList = new()
        {
            "¬раг в €рости и атакует бездумно"
        };
        int fireOrBase = random.Next(1, 3);
        if (fireOrBase != 1 && CombatSystem.instance.enemyUnit.currentMP >= 3)
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
                int totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add(CombatSystem.instance.playerUnit.ApplyEffect(2));
                messageList.Add("¬раг наносит " + totalDamage + " огненного урона");
            }
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                messageList.Add(CombatSystem.instance.ReflectAction(CombatSystem.instance.enemyUnit, -1, (int)(-1.15 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.enemyUnit)), out _));
                CombatSystem.instance.enemyIsHurting = true;
            }
            else
            {
                int totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.playerIsHurting = true;
                messageList.Add("¬раг наносит " + totalDamage + " физического урона");
            }
        }
        return messageList;
    }
}
