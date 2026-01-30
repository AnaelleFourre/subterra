using UnityEngine;

public class DeleteSaveButton : MonoBehaviour
{
    public GameObject button;
    public int number;

    private void Update()
    {
        var data = SaveManager.SlotData(number);
        var dataIsPresent = data != null;
        button.SetActive(dataIsPresent);
    }

    // public void DeleteSave()
    // {
    //     var data = SaveManager.SlotData(number);
    //     if (data != null)
    //         SaveManager.DeleteSave(number);
    // }
}
