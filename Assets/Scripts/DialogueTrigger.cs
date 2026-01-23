using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player") || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player")))
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue();
                hasTriggered = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player")))
        {
            hasTriggered = false;
        }
    }
}