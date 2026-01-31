using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    private bool inTrigger = false;

    private bool IsPlayer(Collider other) => other.CompareTag("Player") || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"));

    private void OnTriggerEnter(Collider other)
    {
        if (inTrigger || !IsPlayer(other) || DialogueManager.Instance == null || dialogueBox == null)
            return;
        DialogueManager.Instance.ShowDialogue(dialogueBox);
        inTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;
        inTrigger = false;
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.HideDialogue();
    }
}