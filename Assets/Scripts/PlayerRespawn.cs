using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 startPosition;

    void Start()
    {

        startPosition = transform.position;
    }

    public void RestartPlayer()
    {

        transform.position = startPosition;


        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}