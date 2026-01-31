using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private float startTime = 30f;
    [SerializeField] private TextMeshProUGUI timerText;
    private float timeRemaining;

    private void Awake()
    {
        timeRemaining = startTime;
        Debug.Log(startTime + " " + timeRemaining);
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
            RestartGame();
    }

    private void DisplayTime(float time)
    {
        if (timerText == null) return;
        var minutes = Mathf.FloorToInt(time / 60);
        var seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void RestartGame()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            RespawnManager.RespawnStart(player);
        timeRemaining = startTime;
    }
}
