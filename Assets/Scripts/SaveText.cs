using UnityEngine;
using TMPro;

/**
* Classe pour l'affichage dynamique du texte sur les boutons sauvegardes du menu principal
*/
public class SaveText : MonoBehaviour
{

    public TextMeshProUGUI texteUI;
    public int number; // Le numéro de la sauvegarde

    void Start()
    {
        SaveManager.currentSlot = number;
        SaveData data = SaveManager.SlotInfo();
        string texte;
        if (data == null){
            texte = "Sauvegarde vide";
            texteUI.text = texte;
            return;
        }


        texte = data.saveName;
        texte += "\nTemps : " + data.playTime;
        texte += "\nNiveau : " + data.levelNumber;
        texteUI.text = texte;
    }

}
