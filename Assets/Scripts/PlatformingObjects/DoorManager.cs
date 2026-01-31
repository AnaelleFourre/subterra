using UnityEngine;
using Unity.Cinemachine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera doorCamera;
    [SerializeField] private Transform cameraTarget;
    private int defaultCameraPriority;
    public bool isOpen = false;
    private readonly float openAngle = -90f;
    private readonly float duration = 2f;
    private bool isAnimating = false;
    private float timer = 0f;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private PlayerMovement playerControls;

    private void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0f, openAngle, 0f);
        playerControls = FindFirstObjectByType<PlayerMovement>();
        if (doorCamera != null)
            defaultCameraPriority = doorCamera.Priority;
    }

    private void Update()
    {
        if (!isAnimating)
            return;

        timer += Time.deltaTime;
        var t = timer / duration;

        transform.localRotation = isOpen
            ? Quaternion.Lerp(closedRotation, openRotation, t)
            : Quaternion.Lerp(openRotation, closedRotation, t);

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

    private void StartCinematic()
    {
        if (playerControls != null)
        {
            playerControls.LockWalkAnimation();
            playerControls.enabled = false;
        }
        if (doorCamera != null && cameraTarget != null)
        {
            doorCamera.transform.SetPositionAndRotation(cameraTarget.position, cameraTarget.rotation);
            doorCamera.Priority = 20;
        }
    }

    private void EndCinematic()
    {
        if (playerControls != null)
        {
            playerControls.UnlockWalkAnimation();
            playerControls.enabled = true;
        }
        if (doorCamera != null)
            doorCamera.Priority = defaultCameraPriority;
    }
}
