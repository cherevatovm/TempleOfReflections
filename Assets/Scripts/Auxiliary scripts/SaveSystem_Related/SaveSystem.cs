using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static void Save(Player player)
    {
        string path = Path.Combine(Application.persistentDataPath, "/SaveData/SavedGame.json");
        //string json = JsonUtility.ToJson(new SavedData(player));
        File.WriteAllText(path, json);
    }
}
