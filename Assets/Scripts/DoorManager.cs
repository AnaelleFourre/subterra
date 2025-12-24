using UnityEngine;
using Unity.Cinemachine;

public class DoorManager : MonoBehaviour
{
    public bool isOpen = false;
    float openAngle = -90f;
    float duration = 2f;
    bool isAnimating = false;
    float timer = 0f;

    [SerializeField] private CinemachineCamera doorCamera;
    [SerializeField] private Transform cameraTarget;
    PlayerMovement playerControls;
    private int defaultCameraPriority;

    Quaternion closedRotation;
    Quaternion openRotation;
    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0f, openAngle, 0f);
        playerControls = FindFirstObjectByType<PlayerMovement>();

        if (doorCamera != null)
        {
            defaultCameraPriority = doorCamera.Priority;
        }
    }

    void Update()
    {
        if (!isAnimating)
        {
            return;
        }

        timer += Time.deltaTime;
        float t = timer / duration;

        transform.localRotation = Quaternion.Lerp(isOpen ? closedRotation : openRotation, isOpen ? openRotation : closedRotation, t);

        if (t >= 1f)
        {
            isAnimating = false;
            transform.localRotation = isOpen ? openRotation : closedRotation;
            EndCinematic();
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        timer = 0f;
        isAnimating = true;

        StartCinematic();
    }

    void StartCinematic()
    {
        if (playerControls != null)
        {
            playerControls.enabled = false;
        }

        if (doorCamera != null && cameraTarget != null)
        {
            doorCamera.transform.position = cameraTarget.position;
            doorCamera.transform.rotation = cameraTarget.rotation;
            doorCamera.Priority = 20;
        }
    }

    void EndCinematic()
    {
        if (playerControls != null)
        {
            playerControls.enabled = true;
        }

        if (doorCamera != null)
        {
            doorCamera.Priority = defaultCameraPriority;
        }
    }
}
