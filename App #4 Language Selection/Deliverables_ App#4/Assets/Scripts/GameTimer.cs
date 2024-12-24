using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text timerText;       // UI Text element to display the time
    private float timeElapsed;   // Keeps track of the elapsed time
    private bool timerRunning;   // Determines if the timer is running

    void Start()
    {
        ResetTimer();  // Initialize and reset the timer at the start
    }

    void Update()
    {
        if (timerRunning)
        {
            // Increment the elapsed time
            timeElapsed += Time.deltaTime;

            // Format and display the time
            timerText.text = FormatTime(timeElapsed);
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
        timerText.text = FormatTime(timeElapsed);
        timerRunning = true; // Restart the timer after reset
    }

    public float GetTimeElapsed()
    {
        return timeElapsed; // Return the current elapsed time
    }

    public void ToggleTimer(bool isActive)
    {
        timerText.gameObject.SetActive(isActive);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
