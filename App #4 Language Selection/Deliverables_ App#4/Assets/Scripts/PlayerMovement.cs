using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required to use SceneManager

public class PlayerMovement : MonoBehaviour
{

    public GameObject MainObject;
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    public Vector3 respawnPoint;

    // Fall threshold (adjust this value as needed)
    public float fallThreshold = -10f;
    private GameTimer gameTimer; // Reference to the GameTimer script


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Reference to Animator component
        respawnPoint = transform.position; // Set initial spawn point
        gameTimer = FindObjectOfType<GameTimer>(); // Find the GameTimer script in the scene

        if (transform.parent != null)
    {
        Debug.Log("Detaching Penguin from MainObject...");
        transform.SetParent(null); // Detach the Penguin from the parent
    }

    }
void Awake()
{
    // Check if another MainObject already exists
    if (GameObject.FindGameObjectsWithTag("MainObject").Length > 1)
    {
        Debug.LogWarning("Duplicate MainObject detected. Destroying...");
        Destroy(gameObject); // Destroy this instance
        return;
    }

    // Persist this object across scenes
    DontDestroyOnLoad(gameObject);
}




    public void Respawn()
    {
        // Move player back to the starting position
        transform.position = respawnPoint;
        rb.velocity = Vector2.zero; // Reset velocity to avoid sliding after respawn
        gameTimer.ResetTimer(); // Reset the timer when respawning

    }

    void Update()
    {
        // Get horizontal input
        float move = Input.GetAxisRaw("Horizontal"); // -1 (left), 0 (no input), 1 (right)




        // Movement control
        HandleMovement(move);

        // Jump control
        HandleJump();

        // Attack control
        HandleAttack();

        // Slide control
        HandleSlide();

        // Pause menu control
        HandlePauseMenu();

        // Fall detection
        CheckForFall();
    }

    private void HandleMovement(float move)
    {
        if (move != 0)
        {
            // Apply movement based on input
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

            // Set walking animation
            animator.SetBool("isWalking", true);

            // Flip sprite based on movement direction
            if (move < 0)
                transform.localScale = new Vector3(-1, 1, 1); // Face left
            else if (move > 0)
                transform.localScale = new Vector3(1, 1, 1); // Face right
        }
        else
        {
            // Stop horizontal movement when no input
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isWalking", false); // Return to idle state
        }
    }
private void HandleJump()
{
    // Jump only if the jump button is pressed and the penguin is grounded
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        // Apply vertical jump force
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

        // Mark the penguin as not grounded to prevent double-jumps
        isGrounded = false;

        // Trigger jumping animation
        animator.SetBool("isJumping", true);
    }
}


    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1")) // Default attack input (Left Ctrl/Mouse Button)
        {
            animator.SetTrigger("isAttacking"); // Trigger attack animation
        }
    }

    private void HandleSlide()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded) // Sliding when grounded
        {
            animator.SetBool("isSliding", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("isSliding", false);
        }
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




    private void CheckForFall()
    {
        if (transform.position.y < fallThreshold)
        {

        {

            // Reload the scene or reset the player to the respawn point
            RestartGame();
        }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Penguin is grounded");
            animator.SetBool("isJumping", false); // Reset jumping state
        }
         if (collision.gameObject.CompareTag("Spike"))
        {
            RestartGame();
        }
        }
    
     void RestartGame()
    {
        // Reload the current active scene
        LevelManager.Instance.DestroyLevelManager();
        SceneManager.LoadScene("Unity Scene");
    }

    IEnumerator FlashAndRestart()
{
    // Add flashing effect here
    yield return new WaitForSeconds(0.5f);
    RestartGame();
}


}
