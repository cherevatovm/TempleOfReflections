using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data Between Scenes", menuName = "Scriptable Objects/Data Between Scenes")]
public class DataBetweenScenes : ScriptableObject
{
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    public int keysInPossession;
    public int coinsInPossession;
    public List<EnemyInfoPanel.EnemyRecord> currentEnemyRecords = new();
}
