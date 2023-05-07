using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneEnemyAI : EnemyAI
{
    private System.Random random = new();
    public int countWeaknessesTurns;

    public override List<string> CombatAI(out int soundID)
    {
        soundID = -1;
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        List<string> messageList = new();
        if (countWeaknessesTurns == 3 && CombatSystem.instance.enemyUnits.Count < 4)
            return CloneMode();
        else
        {
            CombatSystem.instance.curAllyID = random.Next(0, CombatSystem.instance.allyUnits.Count);
            Unit target = CombatSystem.instance.allyUnits[CombatSystem.instance.curAllyID];
            SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
            if (CombatSystem.instance.reflectionProbability1 > 0 && random.NextDouble() < CombatSystem.instance.reflectionProbability1)
            {
                messageList.Add(CombatSystem.instance.ReflectAction(currentEnemyUnit, -1, -CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, currentEnemyUnit), out _));
                CombatSystem.instance.enemyCombatControllers[CombatSystem.instance.curEnemyID].isHurting = true;
                EnemyInfoPanel.instance.ChangeKnownAffinities(currentEnemyUnit.enemyID, 0);
            }
            else
            {
                int totalDamage = CombatSystem.instance.CalcAffinityDamage(0, false, currentEnemyUnit, target);
                target.TakeDamage(totalDamage);
                CombatSystem.instance.allyCombatControllers[CombatSystem.instance.curAllyID].isHurting = true;
                messageList.Add(currentEnemyUnit.unitName + " наносит " + totalDamage + " физического урона");
            }
        }
        return messageList;
    }

    private List<string> CloneMode()
    {
        Enemy currentEnemyUnit = CombatSystem.instance.enemyUnits[CombatSystem.instance.curEnemyID];
        List<string> messageList = new()
        {
            currentEnemyUnit.unitName + " создает своего клона"
        };
        countWeaknessesTurns = 0;
        messageList.Add(currentEnemyUnit.unitName + " создает своего клона");
        GameObject enemyCombat = Instantiate(currentEnemyUnit.enemyPrefabsForCombat[CombatSystem.instance.curEnemyID],
        CombatSystem.instance.enemyCombatPositions[CombatSystem.instance.curEnemyID + 1]);
        CombatSystem.instance.enemyUnits.Add(enemyCombat.GetComponent<Enemy>());
        CombatSystem.instance.enemyAIs.Add(enemyCombat.GetComponent<EnemyAI>());
        CombatSystem.instance.enemyCombatControllers.Add(enemyCombat.GetComponent<CombatController>());
        CombatSystem.instance.enemyUnits[CombatSystem.instance.enemyUnits.Count - 1].knockedTurnsCount = 0;
        CombatSystem.instance.enemyUnits[CombatSystem.instance.enemyUnits.Count - 1].knockedDownTimeout = 0;
        CombatSystem.instance.enemyHUDs[CombatSystem.instance.enemyUnits.Count - 1].gameObject.SetActive(true);
        CombatSystem.instance.enemyHUDs[CombatSystem.instance.enemyUnits.Count - 1].SetHUD(CombatSystem.instance.enemyUnits[CombatSystem.instance.enemyUnits.Count - 1]);
        CombatSystem.instance.enemyUnits[CombatSystem.instance.enemyUnits.Count - 1].combatHUD =
            CombatSystem.instance.enemyHUDs[CombatSystem.instance.enemyUnits.Count - 1];
        return messageList;
    }
}


