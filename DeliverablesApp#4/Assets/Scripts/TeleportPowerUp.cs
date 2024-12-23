using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;
public class TeleportPowerUp : MonoBehaviour
{
    public Vector3[] teleportPositions; // Predefined teleport locations
    public AudioClip teleportSound; // Sound effect for teleportation

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tank")) // Ensure the object colliding is a tank
        {
            TankMovement tankMovement = other.GetComponent<TankMovement>();
            if (tankMovement != null)
            {
                // Choose a random teleport position
                Vector3 teleportTarget = teleportPositions[Random.Range(0, teleportPositions.Length)];

                // Move the tank to the teleport position
                other.transform.position = teleportTarget;

                // Play teleportation sound
                if (teleportSound != null)
                {
                    AudioSource.PlayClipAtPoint(teleportSound, teleportTarget);
                }

                Destroy(transform.parent.gameObject);
                Debug.Log("Teleport Destroyed");

                Debug.Log($"Tank teleported to {teleportTarget}");

                // Destroy the teleportation power-up after it's used
                Destroy(gameObject);
            }
        }
    }
}
