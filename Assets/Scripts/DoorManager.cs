using UnityEngine;

public class DoorManager : MonoBehaviour
{
    private float openAngle = -90f;
    private float duration = 2f;

    public bool isOpen = false;
    private bool isAnimating = false;
    private float timer = 0f;

    Quaternion closedRotation;
    Quaternion openRotation;
    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        if (!isAnimating)
            return;

        timer += Time.deltaTime;
        float t = timer / duration;

        transform.localRotation = Quaternion.Lerp(
            isOpen ? closedRotation : openRotation,
            isOpen ? openRotation : closedRotation,
            t
        );

        if (t >= 1f)
        {
            isAnimating = false;
            transform.localRotation = isOpen ? openRotation : closedRotation;
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        timer = 0f;
        isAnimating = true;
    }
}
