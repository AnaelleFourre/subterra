using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Transform doorPivot;
    public float openAngle = -90f;
    public float duration = 2f;

    public bool isOpen = false;
    bool isAnimating = false;
    float timer = 0f;

    Quaternion closedRotation;
    Quaternion openRotation;
    void Start()
    {
        doorPivot = transform;
        closedRotation = doorPivot.localRotation;
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
