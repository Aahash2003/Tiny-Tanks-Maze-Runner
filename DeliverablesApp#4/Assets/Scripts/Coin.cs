using UnityEngine;
using Complete;

public class Coin : MonoBehaviour
{
    public AudioClip CoinSound;

    private void OnTriggerEnter(Collider other)
    {
        // Ensure the object that collided has the "Tank" tag
        if (other.CompareTag("Tank"))
        {
            // Play the coin sound if available
            AudioSource audioSource = other.GetComponent<AudioSource>();
            if (audioSource != null && CoinSound != null)
            {
                audioSource.PlayOneShot(CoinSound);
            }

            // Attempt to get the GameManager instance
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                // Try to find the TankManager associated with the colliding tank
                TankManager matchingTank = null;
                foreach (var tank in gameManager.m_Tanks)
                {
                    if (tank.m_Instance == other.gameObject)
                    {
                        matchingTank = tank;
                        break;
                    }
                }

                // Add score if a matching tank is found
                if (matchingTank != null)
                {
                    matchingTank.AddScore(1); // Increment the score by 1
                    Debug.Log($"Player {matchingTank.m_PlayerNumber} collected a coin! New Score: {matchingTank.m_Score}");
                }
            }
            else
            {
                Debug.LogError("GameManager instance not found in the scene.");
            }

            // Increment the static coin count in MazeSpawner
            MazeSpawner.TotalCoinsCollected++; // Update the coin count globally
            Debug.Log($"Total Coins Collected: {MazeSpawner.TotalCoinsCollected}");

            // Destroy the coin after it is collected
            Destroy(gameObject);
        }
    }
}
