using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDestroyEvent : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private GameObject prefabToAddToParty;
    [SerializeField] private Transform spawnPlace;
    [SerializeField] private GameObject objectToDestroy;
    [SerializeField] private Unit[] eventTriggers;

    private void Update()
    {
        SpawnPrefab();
    }

    private void SpawnPrefab()
    {
        if (!System.Array.Exists(eventTriggers, elem => elem == null))
            return;
        CombatSystem.instance.allyPrefabsForCombat.Add(prefabToAddToParty);
        Instantiate(prefabToSpawn, spawnPlace);
        Destroy(objectToDestroy);
        Destroy(gameObject);
    }
}
