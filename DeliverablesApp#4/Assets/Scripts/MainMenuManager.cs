using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScreen")
        {
            ReassignStartScreenButtons();
        }
        else if (scene.name == "LeaderBoard UI")
        {
            ReassignLeaderboardBackButton();
        }
    }

    private void ReassignStartScreenButtons()
    {
        // Find Start button
        Button startButton = GameObject.Find("Start Button")?.GetComponent<Button>();
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
        }

        // Find Leaderboard button
        Button leaderboardButton = GameObject.Find("LeaderBoard")?.GetComponent<Button>();
        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.RemoveAllListeners();
            leaderboardButton.onClick.AddListener(OpenLeaderboard);
        }

        Debug.Log("Buttons reassigned in StartScreen");
    }

    private void ReassignLeaderboardBackButton()
    {
        // Find Back button in the LeaderBoard UI scene
        Button backButton = GameObject.Find("BackButton")?.GetComponent<Button>();
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(GoToStartScreen);
        }

        Debug.Log("Back button reassigned in LeaderBoard UI");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void OpenLeaderboard()
    {
        SceneManager.LoadScene("LeaderBoard UI");
    }

    public void GoToStartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
