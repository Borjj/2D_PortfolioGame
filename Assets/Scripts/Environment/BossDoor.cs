using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [SerializeField] private GameObject keyUIImage; // Reference to same UI image
    [SerializeField] private GameObject interactPrompt; // Optional UI prompt to press F
    
    private bool playerInRange = false;
    private PlayerController_2D currentPlayer = null;


// --------------------------------------------------------------------------------------- //

    private void Start()
    {
        // If you have an interact prompt, hide it initially
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Check for F key press only if player is in range and has the key
        if (playerInRange && currentPlayer != null && currentPlayer.HasBossKey())
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenDoor();
            }
        }
    }

// ----------------------------------------------------------------------------------------- //
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.GetComponent<PlayerController_2D>();

            // Show prompt if player has key
            if (interactPrompt != null && currentPlayer.HasBossKey())
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            currentPlayer = null;

            // Hide prompt
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }

    private void OpenDoor()
    {
        // Hide key from UI
        if (keyUIImage != null)
        {
            keyUIImage.SetActive(false);
        }

        // Hide prompt if you have one
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
        
        // Remove key from player
        currentPlayer.UseBossKey();
        
        // Destroy or deactivate door
        Destroy(gameObject);
        // Alternative: gameObject.SetActive(false);
    }
}