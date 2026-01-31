using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteSaveButton : MonoBehaviour
{
    [SerializeField] private int number;
    [SerializeField] private GameObject fallbackSelection;

    private void Update()
    {
        var data = SaveManager.SlotData(number);
        gameObject.SetActive(data != null);
    }

    public void DeleteSave()
    {
        SaveManager.DeleteSave(number);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(fallbackSelection);
    }
}
