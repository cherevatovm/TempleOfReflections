using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoPanel : MonoBehaviour
{
    private class EnemyStats
    {
        public string name;
        //public string description;
        public bool[] knownAffinities = new bool[4];
        public bool[] weaknesses;
        public bool[] resistances;
        public bool[] nulls;

        public EnemyStats(Enemy enemy)
        {
            name = enemy.unitName;
            weaknesses = enemy.weaknesses;
            resistances = enemy.resistances;
            nulls = enemy.nulls;
        }

        public bool this[int i]
        {
            get => knownAffinities[i];
            set => knownAffinities[i] = value;
        }
    }
    [SerializeField] GameObject[] enemyPrefabs;
    private List<EnemyStats> enemyStats;

    private void Start()
    {
        foreach (var prefab in enemyPrefabs)
            enemyStats.Add(new EnemyStats(prefab.GetComponent<Enemy>()));
    }

    public void ChangeKnownAffinities(int enemyID, int damageTypeID) => enemyStats[enemyID][damageTypeID] = true;
}
