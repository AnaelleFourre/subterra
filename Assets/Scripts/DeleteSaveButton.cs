using UnityEngine;

public class DeleteSaveButton : MonoBehaviour
{
    [SerializeField] private int number;

    private void Update()
    {
        var data = SaveManager.SlotData(number);
        var dataIsPresent = data != null;
        gameObject.SetActive(dataIsPresent);
    }

    public void DeleteSave() => SaveManager.DeleteSave(number);
}
