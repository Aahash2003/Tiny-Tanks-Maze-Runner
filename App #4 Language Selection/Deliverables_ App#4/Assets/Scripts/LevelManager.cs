using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Text levelText; // Drag the Text UI object here
    private int currentLevel = 1;     // Track the current level
    private float levelStartTime;

    public float leveltime;

    void Awake()
    {
        // Singleton pattern to ensure one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void Start()
{
    GameTimer gameTimer = FindObjectOfType<GameTimer>();
    if (gameTimer != null)
    {
        gameTimer.ResetTimer();
    }
    else
    {
        Debug.LogWarning("GameTimer not found in the scene.");
    }

    UpdateLevelText();
}


     void OnDestroy()
    {
        // Unsubscribe from sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void DestroyLevelManager()
{
    Debug.Log("Destroying LevelManager...");
    SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from sceneLoaded
    Destroy(gameObject); // Destroy the LevelManager instance
}


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // Check if the current scene is the Start Screen
    if (scene.name == "StartScreen")
    {
        Debug.Log("Start Screen loaded. Destroying LevelManager instance.");
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event
        Destroy(gameObject); // Destroy the LevelManager
        return; // Exit to prevent further processing
    }

    // Try to find the levelText again in the newly loaded scene
    levelText = GameObject.FindWithTag("LevelText")?.GetComponent<Text>();

    // Update the level text if the component exists
    if (levelText != null)
    {
        UpdateLevelText();
    }
    else
    {
        Debug.LogWarning("LevelText UI not found in the new scene.");
    }
}



    public int GetCurrentLevel()
    {
        return currentLevel;
    }


    public float GetLevelTime()
    {
        return Time.time - levelStartTime; // Calculate elapsed time
    }

    public void IncrementLevel()
{
    if (currentLevel >= 2)
    {
        Debug.LogWarning("Cannot increment level beyond Level 2.");
        return; // Prevent level from incrementing past 2
    }

    GameTimer gameTimer = FindObjectOfType<GameTimer>();
    if (gameTimer != null)
    {
        leveltime = gameTimer.GetTimeElapsed();
        Debug.Log($"Level {currentLevel} completed in {gameTimer.GetTimeElapsed()} seconds.");
        gameTimer.ResetTimer(); // Reset the timer for the next level
    }

    currentLevel++;
    UpdateLevelText();
}



    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
        else
        {
            Debug.LogWarning("Level Text UI is not assigned in the Inspector.");
        }
    }

    public void ShowLevelComplete()
    {
        if (currentLevel > 2)
        {
            SceneManager.LoadScene("Win Scene");
        }
        Debug.Log("Level Complete! Level " + currentLevel);
    }

    public void ResetGame()
    {
        currentLevel = 1; // Reset level
        UpdateLevelText(); // Update the UI
    }
}
