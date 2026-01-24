using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; 

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private GameObject activeBox;
    private bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else { 
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isDialogueActive && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            HideDialogue();
        }
    }

    public void ShowDialogue(GameObject boxToUse, params TextMeshProUGUI[] textsToShow)
    {
        if (activeBox != null && activeBox != boxToUse) 
            activeBox.SetActive(false);

        activeBox = boxToUse;
        
        if (activeBox != null)
        {
            activeBox.SetActive(true);

            if (textsToShow != null)
            {
                foreach (TextMeshProUGUI txt in textsToShow)
                {
                    if (txt != null)
                    {
                        txt.gameObject.SetActive(true);
                    }
                }
            }

            isDialogueActive = true;
        }
    }

    public void HideDialogue()
    {
        if (activeBox != null)
            activeBox.SetActive(false);
        
        isDialogueActive = false;
    }
}