using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public static TerrainSpawner Instance;
    public GameObject flagPrefab;         // Flag prefab
    public GameObject terrainPrefab;      // Terrain block prefab
    public GameObject trapPrefab;         // Trap prefab
    public float spawnInterval = 2f;      // Time interval between each terrain spawn
    public float terrainWidth = 5f;       // Width of each terrain block
    public float baseTrapSpawnChance = 0.5f;  // Base chance of spawning a trap (0 to 1)

    private float spawnTimer;
    private float lastSpawnX;             // Tracks the x-position of the last spawned terrain
    private int terrainCount = 0;         // Tracks the number of terrains spawned
    private int trapsPerLevel = 1;        // Initial number of traps per level
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        ResetTerrain(); // Initialize terrain
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnTerrainWithTrap();
            spawnTimer = 0f;
        }
    }

    public void ResetTerrain()
    {
        Debug.Log("Resetting Terrain...");
        terrainCount = 0; // Reset terrain count
        lastSpawnX = transform.position.x; // Reset spawn position

        UpdateTrapSettings(); // Adjust traps for the current level
    }

    private void UpdateTrapSettings()
    {
        // Get the current level from the LevelManager
        int currentLevel = LevelManager.Instance.GetCurrentLevel();

        // Increase the number of traps with the level, up to a maximum of 5 traps per terrain
        trapsPerLevel = Mathf.Clamp(currentLevel, 1, 5);

        // Adjust the trap spawn chance based on the level
        baseTrapSpawnChance = Mathf.Clamp(0.5f + (currentLevel * 0.05f), 0.5f, 0.9f); // Maximum 90%
    }

   void SpawnTerrainWithTrap()
{
    // Spawn the terrain
    Vector3 terrainSpawnPosition = new Vector3(lastSpawnX + terrainWidth, transform.position.y, 0);
    GameObject newTerrain = Instantiate(terrainPrefab, terrainSpawnPosition, Quaternion.identity);

    // Check if the cloned terrain has a collider
    Collider2D terrainCollider = newTerrain.GetComponent<Collider2D>();
    if (terrainCollider == null)
    {
        Debug.LogError($"Cloned terrain at {terrainSpawnPosition} is missing a Collider2D!");
    }

    // Assign disappearing behavior to some terrains
    if (Random.value < 0.3f) // 30% chance to make terrain disappear/reappear
    {
        DisappearingTerrain disappearingTerrain = newTerrain.GetComponent<DisappearingTerrain>();
        if (disappearingTerrain != null)
        {
            disappearingTerrain.StartDisappearing();
        }
    }

    // Update lastSpawnX
    lastSpawnX = terrainSpawnPosition.x;

    terrainCount++;

    // Spawn traps on the terrain
    for (int i = 0; i < trapsPerLevel; i++)
    {
        if (Random.value < baseTrapSpawnChance)
        {
            Vector3 trapPosition = terrainSpawnPosition + trapPrefab.transform.localPosition;
            GameObject newTrap = Instantiate(trapPrefab, trapPosition, Quaternion.identity);
            
            newTrap.transform.parent = newTerrain.transform;
            // Check if the cloned trap has a collider
            Collider2D trapCollider = newTrap.GetComponent<Collider2D>();
            if (trapCollider == null)
            {
                Debug.LogError($"Cloned trap at {trapPosition} is missing a Collider2D!");
            }
        }
    }

    // Spawn the flag on the 15th terrain
    if (terrainCount == 10)
    {
        Vector3 flagPosition = terrainSpawnPosition + new Vector3(0, 1f, 0); // Adjust height of flag
        Instantiate(flagPrefab, flagPosition, Quaternion.identity);

        Debug.Log("End Point Reached!");
    }
}

}
