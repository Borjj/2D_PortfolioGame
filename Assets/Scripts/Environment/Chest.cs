using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject endKey;
    [SerializeField] private GameObject interactPrompt; // Optional UI prompt to press F
    
    private bool playerInRange = false;
    private PlayerController_2D currentPlayer = null;


// --------------------------------------------------------------------------------------- //

    private void Start()
    {

        endKey.SetActive(false);

        // If you have an interact prompt, hide it initially
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Check for F key press only if player is in range and has the key
        if (playerInRange && currentPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenChest();
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
            if (interactPrompt != null)
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

    private void OpenChest()
    {
        endKey.SetActive(true);

        // Hide prompt if you have one
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
        
        // Destroy or deactivate door
        Destroy(gameObject);
    }
}