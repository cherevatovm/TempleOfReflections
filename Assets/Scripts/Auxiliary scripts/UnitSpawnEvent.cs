using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnEvent : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private GameObject prefabToAddToParty;
    [SerializeField] private Transform spawnPlace;
    [SerializeField] private GameObject[] objectsToDestroy;
    [SerializeField] private Unit[] eventTriggers;

    private void Update()
    {
        if (System.Array.Exists(eventTriggers, elem => !elem.gameObject.activeSelf))
            SpawnPrefab();
    }

    public void SpawnPrefab()
    {
        CombatSystem.instance.allyPrefabsForCombat.Add(prefabToAddToParty);
        Instantiate(prefabToSpawn, spawnPlace);
        foreach (GameObject obj in objectsToDestroy)
            Destroy(obj);
        Destroy(gameObject);
    }
}
