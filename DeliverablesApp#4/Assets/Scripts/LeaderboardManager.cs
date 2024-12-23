using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Complete;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>(); // Store leaderboard entries
    private bool hasCompletedFirstLevel = false;           // Tracks if the first level is completed
    private string leaderboardFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist data across scenes
            leaderboardFilePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
            LoadLeaderboard();

        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    // Adds a new entry to the leaderboard
  public void AddToLeaderboard(string playerName, float roundTime)
{
    if (!string.IsNullOrEmpty(playerName))
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, roundTime);
        leaderboard.Add(newEntry);

        // Sort by best round time (ascending order)
        leaderboard.Sort((a, b) => a.bestRoundTime.CompareTo(b.bestRoundTime));

        SaveLeaderboard();

        Debug.Log($"Added {playerName} to leaderboard with best round time: {roundTime}");
    }
    else
    {
        Debug.LogWarning("Player name is empty or null. Skipping leaderboard entry.");
    }
}



    // Returns the leaderboard entries
    public List<LeaderboardEntry> GetLeaderboard()
    {
        return leaderboard;
    }

    private void SaveLeaderboard()
    {
        try
        {
            string json = JsonUtility.ToJson(new LeaderboardData(leaderboard), true);
            File.WriteAllText(leaderboardFilePath, json);
            Debug.Log("Leaderboard saved to " + leaderboardFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save leaderboard: " + e.Message);
        }
    }

    private void LoadLeaderboard()
    {
        try
        {
            if (File.Exists(leaderboardFilePath))
            {
                string json = File.ReadAllText(leaderboardFilePath);
                LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
                leaderboard = data.entries ?? new List<LeaderboardEntry>();
                Debug.Log("Leaderboard loaded from " + leaderboardFilePath);
            }
            else
            {
                Debug.Log("No leaderboard file found. Starting fresh.");
                leaderboard = new List<LeaderboardEntry>();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load leaderboard: " + e.Message);
            leaderboard = new List<LeaderboardEntry>();
        }
    }




    // Marks that the first level has been completed
    public void CompleteFirstLevel()
    {
        hasCompletedFirstLevel = true;
    }

    // Checks if the first level is completed
    public bool IsFirstLevelCompleted()
    {
        return hasCompletedFirstLevel;
    }

    // Load the LeaderBoard UI scene
    public void LoadLeaderboardScene()
    {
        SceneManager.LoadScene("LeaderBoard UI");
    }

    

    // Load the main game scene
    public void LoadGameScene()
    {
        SceneManager.LoadScene("StartScreen"); // Adjust based on your main game scene's name
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float bestRoundTime; // Fastest round time

    public LeaderboardEntry(string playerName, float bestRoundTime)
    {
        this.playerName = playerName;
        this.bestRoundTime = bestRoundTime;
    }
}


[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;

    public LeaderboardData(List<LeaderboardEntry> entries)
    {
        this.entries = entries;
    }
}