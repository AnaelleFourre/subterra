
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private float startTime = 30f;
    [SerializeField] private TextMeshProUGUI timerText;
    private float timeRemaining;

    void Awake()
    {
        timeRemaining = startTime;
        Debug.Log(startTime + " " + timeRemaining);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            RestartGame();
        }
    }

    void DisplayTime(float time)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void RestartGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnManager.RespawnStart(player);
        }

        timeRemaining = startTime;
    }
}

