using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveButton : MonoBehaviour
{

    public GameObject button;
    public int number;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        SaveManager.SetSlot(number);
        SaveData data = SaveManager.SlotInfo();
        if (data == null){
            button.SetActive(false);
            return;
        }
        
    }

    public void DeleteSave()
    {
        SaveManager.SetSlot(number);
        SaveData data = SaveManager.SlotInfo();
        if (data != null)
        {
            SaveManager.DeleteSave(number);   
        }
    }
}
