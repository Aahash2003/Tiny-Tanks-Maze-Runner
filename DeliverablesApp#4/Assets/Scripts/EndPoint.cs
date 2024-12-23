using UnityEngine;
using UnityEngine.SceneManagement; // Required to transition between scenes

public class EndPoint : MonoBehaviour
{
    private bool levelCompleted = false; // Prevent multiple triggers

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted) return; // Exit if level completion logic has already been triggered

        if (other.CompareTag("Player"))
        {
            Debug.Log("Level Complete! Penguin touched the flag.");
            levelCompleted = true; // Mark level as completed

            // Check if the current level is 2
            if (LevelManager.Instance.GetCurrentLevel() == 2)
            {
                // End the game and transition to the Win Scene
                Debug.Log("Game Complete! Transitioning to Win Scene...");
                SceneManager.LoadScene("Win Scene");
            }
            else
            {
                // Increment level and reset for the next level
                LevelManager.Instance.IncrementLevel();
                TerrainSpawner.Instance.ResetTerrain();
                other.transform.position = new Vector3(0, 0, 0); // Reset player position

                // Allow the next level flag to be activated
                Invoke(nameof(ResetLevelFlag), 0.1f);
            }
        }
    }

    private void ResetLevelFlag()
    {
        levelCompleted = false; // Reset the flag for the next level
    }
}
