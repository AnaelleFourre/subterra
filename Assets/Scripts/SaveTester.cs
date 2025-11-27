using UnityEngine;
using UnityEngine.InputSystem;

public class SaveTester : MonoBehaviour
{
    // Save files in \Users\<User>\AppData\LocalLow\DefaultCompany\Subterra\ + save_0.json / save_1.json / save_2.json
    void Update()
    {
        // Change slot (0, 1 or 2) -> 1 (&), 2 (é), 3 (") on keyboard
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SaveManager.SetSlot(0);
            Debug.Log("→ Slot 0");
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SaveManager.SetSlot(1);
            Debug.Log("→ Slot 1");
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SaveManager.SetSlot(2);
            Debug.Log("→ Slot 2");
        }

        // Init -> i
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (GameObject.FindWithTag("Player") == null) Debug.Log("Plyer null");
            SaveManager.InitSave("TestSave", GameObject.FindWithTag("Player").transform.position);
            Debug.Log("→ Init du slot");
        }

        // Save -> o
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            SaveManager.Save(GameObject.FindWithTag("Player").transform.position, 2);
            Debug.Log("→ Sauvegarde : " + GameObject.FindWithTag("Player").transform.position);
        }

        // Load -> p
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (SaveManager.Load())
            {
                GameObject.FindWithTag("Player").transform.position = SaveManager.currentData.playerPosition;
                Debug.Log($"→ Chargé, temps : {SaveManager.currentData.playTime}");
            }
        }

        // Info -> u
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            var info = SaveManager.SlotInfo();
            if (info == null)
                Debug.Log("→ Slot vide");
            else
                Debug.Log($"→ Info : {info.saveName}, {info.playTime}, {info.playerPosition}, {info.levelNumber}");
        }

        // Delete -> y
        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            SaveManager.DeleteSave(0);
            Debug.Log($"→ Sauvegarde supprimée slot {0}");
        }
    }
}
