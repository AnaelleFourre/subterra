using UnityEngine;
using System.IO;

public class SaveManager
{
    public static SaveData currentData = new SaveData();
    public static int currentSlot = 0;
    private static readonly string[] paths = new string[3];

    static SaveManager()
    {
        for (int i = 0; i < 3; i++)
        {
            paths[i] = Application.persistentDataPath + $"/save_{i}.json";
        }
    }

    public static void Save(Vector3 position)
    {
        var path = paths[currentSlot];

        currentData.playerPosition = position;
        currentData.playTime = TimeManager.instance.playTime;

        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(path, json);

        Debug.Log("Sauvegarde effectuée en " + path);
    }

    public static bool Load()
    {
        var path = paths[currentSlot];

        if (!File.Exists(path))
        {
            Debug.LogWarning(path + ": aucune sauvegarde trouvée");
            return false;
        }

        string json = File.ReadAllText(path);
        currentData = JsonUtility.FromJson<SaveData>(json);

        TimeManager.instance.SetPlayTime(currentData.playTime);

        return true;
    }

    public static void InitSave(string saveName, Vector3 startPosition)
    {
        var path = paths[currentSlot];

        if (!File.Exists(path))
        {
            SaveData data = new SaveData();
            data.saveName = saveName;
            data.playerPosition = startPosition;
            data.playTime = 0;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            TimeManager.instance.SetPlayTime(0);

            currentData = data;

            Debug.Log("Sauvegarde " + saveName + " initialisée");
        }
    }

    public static SaveData SlotInfo()
    {
        var path = paths[currentSlot];

        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void SetSlot(int newSlot)
    {
        if (newSlot >= 0 && newSlot < 3)
        {
            currentSlot = newSlot;
            Debug.Log("Changement de slot : " + newSlot);
        }
    }
}

