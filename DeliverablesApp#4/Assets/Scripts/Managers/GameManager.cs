using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public const int m_NumRoundsToWin = 2; 
        public float m_StartDelay = 3;             // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_EndDelay = 3;               // The delay between the end of RoundPlaying and RoundEnding phases.
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.
        public Text m_Player1ScoreText; // UI text to display Player 1's score.
        public Text m_Player2ScoreText; // UI text to display Player 2's score.
        private bool m_IsRoundEnding = false;
        public bool GameWonIndicator = false;
        public GameObject ScorePanel; // Reference to the panel that holds the score texts
        public GameObject ScoreTextPrefab; // Reference to the score text prefab

        private int m_RoundNumber;                  // Which round the game is currently on.
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

        private float m_RoundStartTime; // Time when the round starts
        private float m_RoundDuration;  // Duration of the current round
        private float m_FastestRound = Mathf.Infinity; // Fastest round time (initialize to infinity)

        // Property to get the fastest round time (for leaderboard)
        public float FastestRoundTime => m_FastestRound;

        
        private bool m_IsGameLoopRunning = false;


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
        }
    }
        private void Start()
        {
            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);
            

            SpawnAllTanks();
            SetCameraTargets();
            InitializeScoreUI(); // Set up score UI for the tanks
            UpdateScoreUI(); // Initialize the score display.

            // Once the tanks have been created and the camera is using them as targets, start the game.
            StartCoroutine (GameLoop ());
        }

        void Update(){
            HandlePauseMenu();

        }
public string ISU(){
    InitializeScoreUI();
    return "Score UI initialized.";
}
private void InitializeScoreUI()
{
    if (ScorePanel == null || ScoreTextPrefab == null || m_Tanks == null)
    {
        Debug.LogError("ScorePanel, ScoreTextPrefab, or m_Tanks is not assigned in the GameManager.");
        return;
    }

    foreach (var tank in m_Tanks)
    {
        if (tank == null)
        {
            Debug.LogError("TankManager instance is null.");
            continue;
        }

        // Create a new score text UI element
        GameObject scoreTextObject = Instantiate(ScoreTextPrefab, ScorePanel.transform);

        // Assign the text with localization
        Text scoreText = scoreTextObject.GetComponent<Text>();
        if (scoreText != null)
        {
            scoreText.text = LocalizationManager.Instance != null
                ? LocalizationManager.Instance.GetLocalizedString("Player", tank.m_PlayerNumber, 0)
                : $"Player {tank.m_PlayerNumber}: 0";
        }
        else
        {
            Debug.LogError("Text component not found on ScoreTextPrefab.");
        }

        // Assign it to the tank's score text field
        tank.m_ScoreText = scoreText;
    }
}

public void EndCurrentRound()
{
    if (m_IsRoundEnding) return; // Prevent multiple calls to EndCurrentRound
    StartCoroutine(EndAndStartNextRound());
}

private IEnumerator EndAndStartNextRound()
{
    // Wait for RoundPlaying to complete
    yield return StartCoroutine(RoundPlaying());

    // Wait for RoundEnding to complete
    yield return StartCoroutine(RoundEnding());

    if (m_GameWinner != null)
    {
        // If there is a game winner, reload the scene
        ReloadScene();
    }
    else
    {
        // If no game winner yet, continue with the game loop
        yield return StartCoroutine(GameLoop());
    }
}




        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... create them, set their player number and references needed for control.
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }

        public void AddScoreToTank(int tankNumber, int score)
{
    if (m_Tanks == null || m_Tanks.Length < tankNumber)
    {
        Debug.LogError($"Invalid tankNumber {tankNumber} or uninitialized m_Tanks array.");
        return;
    }

    if (m_Tanks[tankNumber - 1] == null)
    {
        Debug.LogError($"TankManager for tankNumber {tankNumber} is null.");
        return;
    }

    Debug.Log($"Adding score to Tank {tankNumber}");
    m_Tanks[tankNumber - 1].AddScore(score);
    UpdateScoreUI();
}



        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[m_Tanks.Length];

            // For each of these transforms...
            for (int i = 0; i < targets.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }


        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop ()
        {
            if (m_IsGameLoopRunning) yield break;

            m_IsGameLoopRunning = true;
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine (RoundStarting ());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine (RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine (RoundEnding());

             m_IsGameLoopRunning = false;

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            if (m_GameWinner != null)
            {
                // If there is a game winner, restart the level.
                 ReloadScene();
        
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues.
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                StartCoroutine (GameLoop ());
            }
        }

    
public void reload(){
    ReloadScene();
}

        private void ReloadScene()
{
     m_FastestRound = Mathf.Infinity; // Reset fastest round time
    // Reload the current scene to reset everything
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

public void ResetGameState()
{
    // Get the MazeSpawner instance
    MazeSpawner mazeSpawner = FindObjectOfType<MazeSpawner>();
    if (mazeSpawner != null)
    {
        mazeSpawner.RespawnWalls();
        mazeSpawner.RespawnCoins();
        mazeSpawner.RespawnHearts();
        mazeSpawner.RespawnTeleportPowerUp();
    }

    // Reset other game elements, like walls and player states
    m_Tanks[0].ClearScore();
    m_Tanks[1].ClearScore();
    ResetAllTanks();
    UpdateScoreUI(); // Reset UI if needed
}


        private IEnumerator RoundStarting()
{
    // Reset all tanks and disable control.
    ResetAllTanks();
    DisableTankControl();
    ResetGameState();

    // Reset the round winner for a fresh start.
    m_RoundWinner = null;

    // Snap the camera's zoom and position to something appropriate for the reset tanks.
    m_CameraControl.SetStartPositionAndSize();

    // Increment the round number and display text showing the players what round it is.
    m_RoundNumber++;
    m_MessageText.text = LocalizationManager.Instance.GetLocalizedString("ROUND_START", m_RoundNumber);
    Debug.Log("Round" + m_RoundNumber);

    m_RoundStartTime = Time.time;


    // Wait for the specified length of time until yielding control back to the game loop.
    yield return m_StartWait;
}


         private void HandlePauseMenu()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (Time.timeScale == 0)
        {
            // Resume the game
            Time.timeScale = 1;

            // Unload the pause menu scene only if it's loaded
            if (SceneManager.GetSceneByName("PauseMenuScene").isLoaded)
            {
                SceneManager.UnloadSceneAsync("PauseMenuScene");
            }
        }
        else
        {
            // Pause the game
            Time.timeScale = 0;

            // Load the pause menu additively
            SceneManager.LoadScene("PauseMenuScene", LoadSceneMode.Additive);
        }
    }
}


      private IEnumerator RoundPlaying()
{
    // Enable tank control as the round starts.
    EnableTankControl();

    // Clear any previous messages from the screen.
    m_MessageText.text = string.Empty;

    // Keep playing until either one tank is left or a round winner is set (e.g., flag interaction).
    while (!OneTankLeft() && m_RoundWinner == null)
    {
        yield return null; // Wait for the next frame to check again.
    }
    

    // If we exit the loop, it means the round conditions have been met.
    Debug.Log("RoundPlaying: Exiting loop due to a winner or single tank left.");

    // Disable tank control at the end of the round.
    DisableTankControl();
}

private IEnumerator RoundEnding()
{
    // Prevent multiple executions of RoundEnding.
    if (m_IsRoundEnding) yield break;

    Debug.Log("RoundEnding started.");
    m_IsRoundEnding = true;

    // Disable control for all tanks during the round-ending phase.
    DisableTankControl();

    // Determine the round winner if not already set.
    if (m_RoundWinner == null)
    {
        m_RoundWinner = GetRoundWinner();
        Debug.Log(m_RoundWinner.m_PlayerNumber + " HI");
    }


    // Increment the winner's win count and handle leaderboard updates.
    if (m_RoundWinner != null)
    {
        Debug.Log($"Round Winner: Player {m_RoundWinner.m_PlayerNumber}");
        m_RoundWinner.m_Wins++;
        Debug.Log("Rounds Won" + m_RoundWinner.m_Wins);
    }

        // Calculate the round duration.
        m_RoundDuration = Time.time - m_RoundStartTime;

        // Save round winner details to PlayerPrefs for leaderboard submission.
        PlayerPrefs.SetString("WinnerName", $"Player {m_RoundWinner.m_PlayerNumber}");
        PlayerPrefs.SetFloat("BestRoundTime", m_RoundDuration);

        // Update the fastest round time if applicable.
        if (m_FastestRound < 0 || m_RoundDuration < m_FastestRound)
        {
            m_FastestRound = m_RoundDuration;
            Debug.Log($"New Fastest Round Time: {m_FastestRound:F2} seconds.");
        }

        Debug.Log($"Round Duration: {m_RoundDuration:F2} seconds.");
    

    // Determine the overall game winner, if any.
    m_GameWinner = GetGameWinner();
    if (m_GameWinner != null)
    {
        Debug.Log($"Game Winner: Player {m_GameWinner.m_PlayerNumber}");

        // Save game-winner data to PlayerPrefs for leaderboard.
        PlayerPrefs.SetString("GameWinnerName", $"Player {m_GameWinner.m_PlayerNumber}");
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("GameWon", 1); // 1 for true, 0 for false
                    PlayerPrefs.Save();
                    Debug.Log("Just checking statements");
                    SceneManager.LoadScene("LeaderBoard UI");

    }

    // Display the round-end message.
    string message = EndMessage();
    m_MessageText.text = message;

    // Wait for the end delay before proceeding.
    yield return m_EndWait;

    // Reset the round-ending flag.
    m_IsRoundEnding = false;
}




        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 1;
        }
        
        
        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
       private TankManager GetRoundWinner()
{
    // Go through all the tanks...
    for (int i = 0; i < m_Tanks.Length; i++)
    {
        // Check if the tank is active
        if (m_Tanks[i].m_Instance.activeSelf)
        {
            Debug.Log($"Player {m_Tanks[i].m_PlayerNumber} current wins: {m_Tanks[i].m_Wins}");
            return m_Tanks[i]; // Return the active tank as the round winner
        }
    }

    // If none of the tanks are active, it's a draw
    Debug.Log("No active tanks found. Round ends in a draw.");
    return null;
}


        // This function is to find out if there is a winner of the game.
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...

            for (int i = 0; i < m_Tanks.Length; i++)
            {
                Debug.Log($"Required Wins to Game Winner: {m_NumRoundsToWin}");
                Debug.Log("Tanks"+m_NumRoundsToWin);
                // ... and if one of them has enough rounds to win the game, return it.
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin){
                    return m_Tanks[i];
                }
            }

            // If no tanks have enough rounds to win, return null.
            return null;
        }

        public void GameWon(){
            PlayerPrefs.SetInt("GameWon", 1); // 1 for true, 0 for false
            PlayerPrefs.Save();


            GameWonIndicator = true;


        }


        private string EndMessage()
{
    string message = LocalizationManager.Instance.GetLocalizedString("DRAW");

    if (m_RoundWinner != null)
        message = LocalizationManager.Instance.GetLocalizedString("WINS_ROUND", m_RoundWinner.m_ColoredPlayerText);

    message += "\n\n\n\n";

    for (int i = 0; i < m_Tanks.Length; i++)
    {
        message += LocalizationManager.Instance.GetLocalizedString("PLAYER_WINS", m_Tanks[i].m_ColoredPlayerText, m_Tanks[i].m_Wins) + "\n";
    }

    if (m_GameWinner != null)
        message = LocalizationManager.Instance.GetLocalizedString("WINS_GAME", m_GameWinner.m_ColoredPlayerText);

    if (m_FastestRound != Mathf.Infinity)
    {
        message += $"\n" + LocalizationManager.Instance.GetLocalizedString("FASTEST_ROUND", m_FastestRound);
    }

    return message;
}


       public void SetRoundWinner(TankManager winner)
{
    m_RoundWinner = winner;
    Debug.Log($"Round Winner set: Player {m_RoundWinner.m_PlayerNumber}");
}






        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }
        }

        public void UpdateScoreUI()
        {
            foreach (var tank in m_Tanks)
            {
                if (tank.m_ScoreText != null)
                {
                    Debug.Log($"USI Player -> PlayerNumber: {tank.m_PlayerNumber}, Score: {tank.m_Score}");

                    
                    tank.m_ScoreText.text = LocalizationManager.Instance.GetLocalizedString("Player", tank.m_PlayerNumber, tank.m_Score);

                }
            }
        }





        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
    }
}


