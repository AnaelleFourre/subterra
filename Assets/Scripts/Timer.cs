
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float startTime = 30f;
    private float timeRemaining;
    public TextMeshProUGUI timerText;

    void Start()
    {
        timeRemaining = startTime;
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
        Debug.Log("Time Is Done ");

        PlayerRespawn player = FindObjectOfType<PlayerRespawn>();
        if (player != null)
        {
            player.RestartPlayer();
        }


        timeRemaining = startTime;
    }
}

