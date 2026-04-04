using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string savePath = Path.Combine(Application.persistentDataPath, "autosave.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json); // overwrites existing autosave
    }

    public static SaveData Load()
    {
        if (!File.Exists(savePath))
            return null;

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }
}