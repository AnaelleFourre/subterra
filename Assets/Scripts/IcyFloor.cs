using UnityEngine;

public class IcyFloor : MonoBehaviour
{
    // 1.0 = Normal Grip (Instant stop/start)
    // 0.5 = Slippery
    // 0.1 = Very Icy
    [Range(0.1f, 10f)]
    public float Grip = 0.3f; 
}