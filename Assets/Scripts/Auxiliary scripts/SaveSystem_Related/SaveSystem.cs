using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static void Save(SavedData savedData)
    {
        string path = Path.Combine(Application.persistentDataPath, "/SavedData/SavedGame.json");
        string json = JsonUtility.ToJson(savedData);
        File.WriteAllText(path, json);
    }

    public static SavedData Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "/SavedData/SavedGame.json");
        SavedData sData = null;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            sData = JsonUtility.FromJson<SavedData>(json);
        }
        return sData;
    }
}
