using UnityEngine;
using Complete;

public class Flag : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Ensure the colliding object is a tank.
        if (other.CompareTag("Tank"))
        {
            // Get the GameManager instance.
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                // Find the matching tank.
                foreach (var tank in gameManager.m_Tanks)
                {
                    if (tank.m_Instance == other.gameObject)
                    {
                        Debug.Log("Flag interaction detected.");

                        Debug.Log($"Player {tank.m_PlayerNumber} captured the flag!");

                        // Set the round winner to the tank that captured the flag.
                        gameManager.SetRoundWinner(tank);

                        // End the round immediately.
                        gameManager.EndCurrentRound();

                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("GameManager instance not found in the scene.");
            }
        }
    }
}
