using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool isInTutorial;
    public bool[] inventoryTutorialSteps = new bool[3];
    public int combatTutorialSteps;
    public bool wasContainerTutorialShown;
    public bool wasGameSaved;

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
        instance.isInTutorial = true;
        //instance.isInTutorial = (SceneManager.GetActiveScene().buildIndex == 1);
    }
}
