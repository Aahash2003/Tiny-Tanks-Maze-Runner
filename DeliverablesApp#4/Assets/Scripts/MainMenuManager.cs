using UnityEngine;
using UnityEngine.SceneManagement; // Required to use SceneManager

public class MainMenuManager : MonoBehaviour
{
    // Function to load the gameplay scene
    public void StartGame()
    {
        // Ensure the scene name matches your gameplay scene

        SceneManager.LoadScene("Game Scene");
    }

    // Function to quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        SceneManager.LoadScene("StartScreen");
    
    }

    public void OpenLeaderboard()
    {
        SceneManager.LoadScene("LeaderBoard UI");
        Debug.Log("Open Leaderboard!");
    }
}
