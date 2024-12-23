using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Complete;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject leaderboardEntryPrefab; // Prefab for leaderboard entries
    public Transform leaderboardContent;      // The Content object of the Scroll View
    public InputField NameInput;             // InputField for the player's name
    public Button SubmitButton;              // Submit button
    public Button BackButton;                // Back button   

    private bool allowNameInput = false;     // Flag to control name input access

    void Start()
    {
        EnableNameInput(false); // Disable input by default

        if (PlayerPrefs.HasKey("WinnerName") && PlayerPrefs.HasKey("BestRoundTime"))
    {
        string winnerName = PlayerPrefs.GetString("WinnerName");
        float bestRoundTime = PlayerPrefs.GetFloat("BestRoundTime");

        // Pre-fill the name input with the winner's name
        NameInput.text = winnerName;

        Debug.Log($"Pre-filled Winner: {winnerName}, Best Round Time: {bestRoundTime:F2}s");
    }


        PopulateLeaderboard(); // Populate leaderboard (may be empty)
        // Start rechecking for LevelManager.Instance periodically
        InvokeRepeating("CheckForLevelManager", 0f, 0.5f);
        BackButton.onClick.AddListener(GoBackToGame);

        

        // Attach button listeners
        SubmitButton.onClick.AddListener(SubmitName);
    }

    void CheckForLevelManager()
{
    
    if (PlayerPrefs.GetInt("GameWon", 0) == 1)
    {
        EnableNameInput(true);
        Debug.Log("Game completed. Enabling name input.");
    }
    else
    {
        Debug.LogWarning("Game is still ongoing.");
    }
}



    public void EnableNameInput(bool enable)
    {
        NameInput.interactable = enable;
        SubmitButton.interactable = enable;
        allowNameInput = enable;
    }

    // Populate leaderboard entries dynamically
    void PopulateLeaderboard()
{
    // Clear existing entries
    foreach (Transform child in leaderboardContent)
    {
        Destroy(child.gameObject);
    }

    // Get leaderboard entries as a List of LeaderboardEntry
    List<LeaderboardEntry> leaderboardEntries = LeaderboardManager.Instance.GetLeaderboard();

    // Populate the leaderboard dynamically
    foreach (LeaderboardEntry entry in leaderboardEntries)
    {
        GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
        Text entryText = newEntry.GetComponent<Text>();

        if (entryText != null)
        {
            entryText.text = $"{entry.playerName}: Best Time {entry.bestRoundTime:F2}s";
        }
    }
}


    public void SubmitName()
{
    if (!allowNameInput) return; // Prevent submissions if input is disabled

    string playerName = NameInput.text;

    if (!string.IsNullOrEmpty(playerName))
    {
        // Retrieve round time from PlayerPrefs
        float bestRoundTime = PlayerPrefs.GetFloat("BestRoundTime", -1);

        if (bestRoundTime >= 0)
        {
            // Add the player's name and round time to the leaderboard
            LeaderboardManager.Instance.AddToLeaderboard(playerName, bestRoundTime);

            Debug.Log($"Added {playerName} to leaderboard with time: {bestRoundTime:F2}s");

            // Clear PlayerPrefs after submission
            PlayerPrefs.DeleteKey("WinnerName");
            PlayerPrefs.DeleteKey("BestRoundTime");
            PlayerPrefs.Save();

            // Clear input, refresh leaderboard, and disable further input
            NameInput.text = "";
            PopulateLeaderboard();
            PlayerPrefs.SetInt("GameWon", 0);
            EnableNameInput(false);
        }
        else
        {
            Debug.LogWarning("Best round time is invalid. Cannot submit to leaderboard.");
        }
    }
}

    
    


    public void GoBackToGame()
    {
        // Disable input and go back to the game scene
        EnableNameInput(false);
        SceneManager.LoadScene("StartScreen");
    }
}
