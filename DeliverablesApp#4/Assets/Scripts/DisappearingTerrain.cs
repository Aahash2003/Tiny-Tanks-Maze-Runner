using UnityEngine;

public class DisappearingTerrain : MonoBehaviour
{
    public float disappearDelay = 2f; // Time before the platform disappears
    public float reappearDelay = 3f; // Time before the platform reappears

    private Renderer[] renderers; // Array of renderers for all child sprites
    private Collider2D[] colliders; // Array of colliders for all child objects

    void Start()
    {
        // Get all renderers and colliders in the hierarchy
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider2D>();

        if (renderers.Length == 0)
        {
            Debug.LogError("No renderers found in DisappearingTerrain!");
        }

        if (colliders.Length == 0)
        {
            Debug.LogError("No colliders found in DisappearingTerrain!");
        }
    }

    public void StartDisappearing()
    {
        Debug.Log($"DisappearingTerrain at {transform.position} will disappear in {disappearDelay} seconds.");
        // Begin disappearing and reappearing cycle
        Invoke(nameof(Disappear), disappearDelay);
        Invoke(nameof(Reappear), disappearDelay + reappearDelay);
    }

    private void Disappear()
    {
        Debug.Log($"DisappearingTerrain at {transform.position} is now disappearing.");

        // Disable all renderers and colliders
        foreach (Renderer rend in renderers)
        {
            rend.enabled = false; // Make the terrain invisible
        }

        foreach (Collider2D col in colliders)
        {
            col.enabled = false; // Disable collisions
        }
    }

    private void Reappear()
    {
        Debug.Log($"DisappearingTerrain at {transform.position} has reappeared.");

        // Enable all renderers and colliders
        foreach (Renderer rend in renderers)
        {
            rend.enabled = true; // Make the terrain visible
        }

        foreach (Collider2D col in colliders)
        {
            col.enabled = true; // Enable collisions
        }
    }
}
