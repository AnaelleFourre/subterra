using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

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

    public static void InitSave(string saveName, string startSceneName, Vector3 startPosition)
    {
        var path = paths[currentSlot];

        if (!File.Exists(path))
        {
            SaveData data = new SaveData
            {
                saveName = saveName,
                playerPosition = startPosition,
                playTime = 0,
                levelNumber = 1,
                sceneName = startSceneName
            };
            data.saveName = saveName;
            data.playerPosition = startPosition;
            data.playTime = 0;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            TimeManager.instance.SetPlayTime(0);

            currentData = data;
        }
    }

    public static void Save(Vector3 position, int levelNumber)
    {
        var path = paths[currentSlot];

        currentData.playerPosition = position;
        currentData.playTime = TimeManager.instance.playTime;
        currentData.levelNumber = levelNumber;
        currentData.sceneName = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(path, json);
    }

    public static bool Load()
    {
        var path = paths[currentSlot];

        if (!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);
        currentData = JsonUtility.FromJson<SaveData>(json);

        TimeManager.instance.SetPlayTime(currentData.playTime);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(currentData.sceneName);

        return true;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentData.playerPosition;
        }
    }

    public static bool DeleteSave(int slot)
    {
        if (slot < 0 || slot > 2)
        {
            return false;
        }

        string path = paths[slot];

        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }

        return false;
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
        }
    }
}

