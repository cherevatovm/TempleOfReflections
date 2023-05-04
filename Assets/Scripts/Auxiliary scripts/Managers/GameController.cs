using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject[] prefabs;
    [HideInInspector] public bool hasBeenLoaded;
    [HideInInspector] public bool isSwitchingScenes;
    [HideInInspector] public SavedData receivedSaveData;
    public static GameController instance;

    private void Awake()
    {        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SoundManager.InitSoundTimerDict();
        }
        else
            Destroy(gameObject);
    }
}
