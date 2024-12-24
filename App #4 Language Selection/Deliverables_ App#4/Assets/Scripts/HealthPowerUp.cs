using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Complete;

public class HealthPowerUp : MonoBehaviour
{
    public int healAmount = 100; // Amount of health the heart restores
     public GameObject heartPrefab; // The prefab of the heart
    public List<Vector3> heartSpawnPositions; // List of positions where hearts will spawn
    private List<GameObject> activeHearts = new List<GameObject>(); // Keep track of active hearts

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tank")) // Check if the object colliding is a tank
        {
            Debug.Log($"{other.name} entered the trigger of the HealthPowerUp.");

            TankHealth tankHealth = other.GetComponent<TankHealth>();
            if (tankHealth != null)
            {
                // Heal the tank
                tankHealth.Heal(healAmount);

                // Play a healing effect (optional)
                PlayHealingEffect();

                // Destroy the heart after collision
                Destroy(transform.parent.gameObject);
                Debug.Log("Heart Destroyed");
            }
        }
    }

    private void PlayHealingEffect()
    {
        // Optionally, add a particle effect or sound effect for feedback
        Debug.Log("Heart collected! Healing effect played.");
    }


}
