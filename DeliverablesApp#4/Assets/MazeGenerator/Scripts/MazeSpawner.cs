using UnityEngine;
using System.Collections.Generic;
using Complete; // Replace with the correct namespace if necessary

public class MazeSpawner : MonoBehaviour {
    public static MazeSpawner Instance;
    public enum MazeGenerationAlgorithm {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }

    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    public bool FullRandom = false;
    public int RandomSeed = 12345;
    public GameObject Floor = null;
    public GameObject Wall = null;
    public GameObject Pillar = null;
    private GameObject wall1, wall2, wall3; // References to the walls to remove.
    public int Rows = 5;
    public int Columns = 5;
    public float CellWidth = 5;
    public float CellHeight = 5;
    public bool AddGaps = true;
    public GameObject CoinPrefab = null;
	public Vector3 TargetPosition = new Vector3(9*5/2, 0, 20); // Adjust to match the wall's position
    public float SearchRadius = 0.1f; // Adjust based on wall size

    private BasicMazeGenerator mMazeGenerator = null;
    public static int TotalCoinsCollected = 0; // Reset this when scene reloads
    private bool goalSpawned = false;  // Track whether the goal is spawned
    private Vector3 goalPosition;  // Store the position for the goal

    public GameObject HeartPrefab = null;
    private List<Vector3> coinPositions = new List<Vector3>(); // Store positions of all coins
    public List<Vector3> heartSpawnPositions; // List of positions where hearts will spawn

    public GameObject teleportPowerUpPrefab = null; // Assign the prefab in the Inspector
public List<Vector3> teleportSpawnPositions; // List of spawn positions for the teleport power-up

	private Complete.GameManager gameManager; // Reference to GameManager
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
    void Start() {

		gameManager = FindObjectOfType<Complete.GameManager>();

		if (gameManager == null)
    {
        Debug.LogError("GameManager not found. Ensure it's added to the scene.");
    }

     // Instantiate the first wall and store its reference.
        wall1 = Instantiate(Wall, new Vector3(12 + (CellWidth + 6) / 20, 0, 20) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
        wall1.transform.parent = transform;

        // Instantiate the third wall and store its reference.
        wall2 = Instantiate(Wall, new Vector3(17 + (CellWidth + 8) / 20, 0, 20) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
        wall2.transform.parent = transform;

        

        wall3 = Instantiate(Wall, new Vector3(22 + (CellWidth - 3) / 4, 0, 20) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
        wall3.transform.parent = transform;

        if (!FullRandom) {
            Random.seed = RandomSeed;
        }

        switch (Algorithm) {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
                break;
        }

        mMazeGenerator.GenerateMaze();

		

        // Generate the maze
        for (int row = 0; row < Rows; row++) {
            for (int column = 0; column < Columns; column++) {
                float x = column * (CellWidth + (AddGaps ? .2f : 0));
                float z = row * (CellHeight + (AddGaps ? .2f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject tmp;

                // Instantiate the floor
                tmp = Instantiate(Floor, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
                tmp.transform.parent = transform;


                

				tmp = Instantiate(Wall, new Vector3(25 - (CellWidth-1)/6, 0, 20 - (CellWidth/2)) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                    tmp.transform.parent = transform;
				
				tmp = Instantiate(Wall, new Vector3(24 + (CellWidth-1)/9, 0, 20 + (CellWidth/2)) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                    tmp.transform.parent = transform;
                
                tmp = Instantiate(Wall, new Vector3(26 + (CellWidth +8) / 20, 0, 20) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
                tmp.transform.parent = transform;

                if (!(row == 0 && column == 0) && cell.WallLeft) {  // Entrance
                    tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;
                    tmp.transform.parent = transform;
                }
                if (!(row == Rows - 1 && column == Columns - 1) && cell.WallRight)  // Exit
{
    Vector3 intendedPosition = new Vector3(x + CellWidth / 2, 0, z);

    // Skip instantiating the wall at the specific position
    if (intendedPosition == new Vector3(22 + (CellWidth-3)/4, 0, 20))
    {
        continue;
    }

    tmp = Instantiate(Wall, intendedPosition + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
    tmp.transform.parent = transform;
}

                if (cell.WallFront) {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                    tmp.transform.parent = transform;
                }
                if (cell.WallBack) {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
                    tmp.transform.parent = transform;
                }

                // Spawn coins
                if (Random.value > 0.8f) {  // Randomly place coins
                    Vector3 coinPosition = new Vector3(x, 1, z);
                    Instantiate(CoinPrefab, coinPosition, Quaternion.identity);
                    RegisterCoinPosition(coinPosition);
                }

                // Save the goal position
                if (cell.IsGoal) {
                    goalPosition = new Vector3(x, 1, z); // Save the goal position for later
                }
            }
        }

        // Instantiate pillars if applicable
        if (Pillar != null) {
            for (int row = 0; row < Rows + 1; row++) {
                for (int column = 0; column < Columns + 1; column++) {
                    float x = column * (CellWidth + (AddGaps ? .2f : 0));
                    float z = row * (CellHeight + (AddGaps ? .2f : 0));
                    GameObject tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 0, z - CellHeight / 2), Quaternion.identity) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }
    }

    void Update() {
        // Check coin collection thresholds and remove walls.
        if (TotalCoinsCollected >= 1 && wall1 != null)
        {
            Destroy(wall1);
            wall1 = null; // Avoid multiple calls to destroy.
            Debug.Log("First wall removed!");
        }

        if (TotalCoinsCollected >= 2 && wall2 != null)
        {
            Destroy(wall2);
            wall2 = null; // Avoid multiple calls to destroy.
            Debug.Log("Second wall removed!");
        }

        if (TotalCoinsCollected >= 3 && wall3 != null)
        {
           Destroy(wall3);
            wall3 = null; // Avoid multiple calls to destroy.
            Debug.Log("Third wall removed!");
        }
        }

    void DestroyWallAtPosition(Vector3 position)
{
    float tolerance = 0.01f; // Adjust as needed for precision
    Collider[] colliders = Physics.OverlapSphere(position, tolerance);

    foreach (Collider collider in colliders)
    {
        // Check if the object is a wall
        if (collider.gameObject.CompareTag("Wall")) // Ensure walls are tagged as "Wall"
        {
            Destroy(collider.gameObject); // Destroy the wall
            Debug.Log($"Wall at {position} destroyed.");
            break; // Exit once the specific wall is destroyed
        }
    }
}    

    private void SpawnGoal() {
        if (CoinPrefab != null) {
            Instantiate(CoinPrefab, goalPosition, Quaternion.identity);
            goalSpawned = true;  // Mark the goal as spawned
        }
    }

	private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Coin")) {
            // Find the tank that collected the coin
            TankManager tankManager = other.GetComponentInParent<TankManager>();
            if (tankManager != null) {
                // Add score to the tank and update GameManager
                Complete.GameManager.Instance.AddScoreToTank(tankManager.m_PlayerNumber, 1);
                TotalCoinsCollected++;
                Destroy(other.gameObject); // Destroy the coin
            }
        }
    }

    public void RespawnWalls(){

        if(wall1 == null){
            wall1 = Instantiate(Wall, new Vector3(12 + (CellWidth + 6) / 20, 0, 20) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
        wall1.transform.parent = transform;
        }
        if (wall2 == null){
            wall2 = Instantiate(Wall, new Vector3(17 + (CellWidth + 8) / 20, 0, 20) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
        wall2.transform.parent = transform;
        }
        if(wall3 ==null){
            wall3 = Instantiate(Wall, new Vector3(22 + (CellWidth -3) / 4, 0, 20) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
        wall3.transform.parent = transform;

        }


    }
    public void RespawnTeleportPowerUp()
{
    // Destroy any remaining teleport power-ups
    foreach (GameObject teleport in GameObject.FindGameObjectsWithTag("Teleport"))
    {
        Destroy(teleport);
    }

    // Ensure the teleport power-up prefab and spawn positions are valid
    if (teleportPowerUpPrefab == null || teleportSpawnPositions == null || teleportSpawnPositions.Count == 0)
    {
        Debug.LogWarning("Teleport power-up prefab or spawn positions are not set!");
        return;
    }

    // Spawn teleport power-ups at all predefined positions
    foreach (Vector3 position in teleportSpawnPositions)
    {
        Instantiate(teleportPowerUpPrefab, position, Quaternion.identity);
        Debug.Log($"Teleport power-up spawned at {position}");
    }

    // Optionally assign the active teleport power-up for additional logic
    Vector3 randomPosition = teleportSpawnPositions[Random.Range(0, teleportSpawnPositions.Count)];
    Debug.Log($"Random teleport power-up spawned at {randomPosition}");
}

    public void RespawnHearts()
    {
        // Destroy any remaining hearts from the previous round
        foreach (GameObject heart in GameObject.FindGameObjectsWithTag("Heart"))
        {
            
                Destroy(heart);
        }

        // Instantiate new hearts at the designated spawn positions
        foreach (Vector3 position in heartSpawnPositions)
        {
            Instantiate(HeartPrefab, position, Quaternion.identity);
        }

        Debug.Log("Hearts respawned at the start of the round.");
    }

   public void RespawnCoins()
    {
        // Clear the TotalCoinsCollected counter
        TotalCoinsCollected = 0;

        // Destroy any remaining coins
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Coin"))
        {
            Destroy(coin);
        }

        // Reinstantiate coins at their original positions
        foreach (Vector3 position in coinPositions)
        {
            Instantiate(CoinPrefab, position, Quaternion.identity);
        }
    }

    public void RegisterCoinPosition(Vector3 position)
    {
        if (!coinPositions.Contains(position))
        {
            coinPositions.Add(position);
        }
    }
}
