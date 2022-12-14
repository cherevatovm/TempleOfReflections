using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake() => instance = this;

    void Start()
    {
        pooledObjects = new();
        GameObject tempObj;
        for (int i = 0; i < amountToPool; i++)
        {
            tempObj = Instantiate(objectToPool);
            tempObj.SetActive(false);
            pooledObjects.Add(tempObj);
        }
    }

    void MakeNonPlayingSoundObjsInactive()
    {
        for (int i = 0; i < amountToPool; i++)
            if (pooledObjects[i].activeInHierarchy && !pooledObjects[i].GetComponent<AudioSource>().isPlaying)
                pooledObjects[i].SetActive(false);
    }

    public GameObject GetLoopedPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
            if (pooledObjects[i].activeInHierarchy && pooledObjects[i].GetComponent<AudioSource>().isPlaying && pooledObjects[i].GetComponent<AudioSource>().loop)
                return pooledObjects[i];
        return null;
    }

    public GameObject GetPooledObject()
    {
        MakeNonPlayingSoundObjsInactive();
        for (int i = 0; i < amountToPool; i++)
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];
        return null;
    }
}
