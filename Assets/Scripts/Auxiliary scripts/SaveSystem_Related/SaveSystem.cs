using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    public static void Save(SavedData savedData)
    {
        string path = Path.Combine(Application.persistentDataPath, "SavedGame.json");
        string json = JsonUtility.ToJson(savedData);
        File.WriteAllText(path, json);
    }

    public static void Load()
    {
        Debug.Log("you tried");
        string path = Path.Combine(Application.persistentDataPath, "SavedGame.json");
        SavedData sData = null;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            sData = JsonUtility.FromJson<SavedData>(json);
            Debug.Log(sData.currentIdol);
            GameController.instance.hasBeenLoaded = true;
            SceneManager.LoadScene(sData.currentSceneIndex);
        }
        GameController.instance.receivedSaveData = sData;
    }
}
