using UnityEngine;

public class LadderSkipper : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<Rigidbody>().MovePosition(targetPosition);
    }
}
