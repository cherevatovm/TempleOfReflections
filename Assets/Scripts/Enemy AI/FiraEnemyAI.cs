using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiraEnemyAI : EnemyAI
{
    System.Random random = new System.Random();

    void Start() => enemyID = 1;

    public override void CombatAI(out string effectMessage)
    {
        effectMessage = string.Empty;
        if (CombatSystem.instance.enemyUnit.currentHP <= (int)(0.33 * CombatSystem.instance.enemyUnit.maxHP))
            StartCoroutine(BerserkMode());
        else
        {
            int attackProbability = random.Next(1, 101);
            if (attackProbability > 75 && CombatSystem.instance.enemyUnit.currentMP >= 3)
            {
                SoundManager.PlaySound(SoundManager.Sound.FiraSkill);
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
                CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
                effectMessage = "���� ������� " + totalDamage + " ��������� �����";
                CombatSystem.instance.playerUnit.FiraEffect();
            }
            else
            {
                SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit);
                CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
                CombatSystem.instance.combatUI.combatDialogue.text = "���� ������� " + totalDamage + " ����������� �����";
            }
        }
    }

    IEnumerator BerserkMode()
    {
        yield return new WaitForSeconds(1f);
        CombatSystem.instance.combatUI.combatDialogue.text = "���� � ������ � ������� ��������";
        yield return new WaitForSeconds(1f);
        int fireOrBase = random.Next(1, 3);
        if (fireOrBase == 1)
        {
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            int totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(0, false, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.combatUI.combatDialogue.text = "���� ������� " + totalDamage + " ����������� �����";
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.FiraSkill);
            int totalDamage = (int)(1.15 * CombatSystem.instance.CalcAffinityDamage(3, true, CombatSystem.instance.enemyUnit, CombatSystem.instance.playerUnit));
            CombatSystem.instance.playerUnit.TakeDamage(totalDamage);
            CombatSystem.instance.enemyUnit.ReduceCurrentMP(3);
            CombatSystem.instance.enemyHUD.ChangeMP(CombatSystem.instance.enemyUnit.currentMP);
            CombatSystem.instance.playerUnit.FiraEffect();
            yield return new WaitForSeconds(1.5f);
            CombatSystem.instance.combatUI.combatDialogue.text = "���� ������� " + totalDamage + " ��������� �����";
        }
    }
}
