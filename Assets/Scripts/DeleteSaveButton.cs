using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveButton : MonoBehaviour
{

    public GameObject button;
    public int number;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        SaveData data = SaveManager.SlotData(number);

        if (data == null)
        {
            button.SetActive(false);
        }
        else
        {
            button.SetActive(true);
        }
    }

    public void DeleteSave()
    {
        SaveData data = SaveManager.SlotData(number);
        if (data != null)
        {
            SaveManager.DeleteSave(number);
        }
    }
}

