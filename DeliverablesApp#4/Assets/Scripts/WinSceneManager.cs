using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviour
{
    public float delayBeforeLeaderboard = 5f; // Delay before transitioning to leaderboard

    void Start()
{
    GameTimer gameTimer = FindObjectOfType<GameTimer>();

    if (gameTimer != null)
    {
        Debug.Log($"Final level completed in {gameTimer.GetTimeElapsed()} seconds.");
        gameTimer.StopTimer(); // Stop the timer
    }

    // Automatically transition to leaderboard
    Invoke("GoToLeaderboard", delayBeforeLeaderboard);
}


    public void GoToLeaderboard()
    {
        Debug.Log("Transitioning to Leaderboard...");
        SceneManager.LoadScene("LeaderBoard UI");
    }
}
