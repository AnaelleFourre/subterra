using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public float playTime = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        playTime += Time.unscaledDeltaTime;
    }

    public void SetPlayTime(float value) => playTime = value;
}
