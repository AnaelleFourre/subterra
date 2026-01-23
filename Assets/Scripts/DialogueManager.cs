using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    private bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else
            Destroy(gameObject);

        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            HideDialogue();
        }
    }

    public void ShowDialogue()
    {
        if (dialogueBox == null) return;

        dialogueBox.SetActive(true);
        isDialogueActive = true;
    }

    public void HideDialogue()
    {
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
        
        isDialogueActive = false;
    }
}