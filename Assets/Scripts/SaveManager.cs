using UnityEngine;
using System.IO;

public class SaveManager
{
    private static string Path => Application.persistentDataPath + "/save_subterra.json";
    public static SaveData currentData = new SaveData();

    public static void Save(Vector3 position, float playTime)
    {
        currentData.playerPosition = position;
        currentData.playTime = playTime;

        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(Path, json);

        Debug.Log("Sauvegarde effectuée en " + Path);
    }

    public static bool Load()
    {
        if (!File.Exists(Path))
        {
            Debug.LogWarning(Path + ": aucune sauvegarde trouvée");
            return false;
        }

        string json = File.ReadAllText(Path);
        currentData = JsonUtility.FromJson<SaveData>(json);
        return true;
    }

    public static void InitSave(string saveName, Vector3 startPosition)
    {
        string path = Application.persistentDataPath + "/save_subterra.json";

        if (!File.Exists(path))
        {
            SaveData data = new SaveData();
            data.saveName = saveName;
            data.playerPosition = startPosition;
            data.playTime = 0;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            Load();

            Debug.Log("Sauvegarde " + saveName + " initialisée");
        }
    }

    public static SaveData SaveInfo()
    {
        string path = Application.persistentDataPath + "/save_subterra.json";

        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}

