using UnityEngine;

public partial class TeleportDoor : MonoBehaviour
{

    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportDestination;
    [SerializeField] private float teleportDelay = 0.5f;  // Optional delay before teleport
    
    private bool playerInRange = false;
    private PlayerController_2D currentPlayer = null;
    private PlayerHealth playerHealth = null;


// --------------------------------------------------------------------------- //

    private void Start()
    {
        // Verify teleport destination is set
        if (teleportDestination == null)
        {
            Debug.LogWarning("No teleport destination set for door: " + gameObject.name);
        }
    }

    private void Update()
    {
        if (playerInRange && currentPlayer != null)
        {
            TeleportPlayer();
            RestartBackgroundMusic();
        }
    }

// --------------------------------------------------------------------------- //


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.GetComponent<PlayerController_2D>();
            playerHealth = other.GetComponent<PlayerHealth>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            currentPlayer = null;
            playerHealth = null;
        }
    }

    private void RestartBackgroundMusic()
    {
        AudioManager.Instance?.RestartMusic();
    }

    private void TeleportPlayer()
    {          
        // Teleport player after delay
        if (teleportDestination != null)
        {
            StartCoroutine(TeleportSequence());
        }  
    }

    private System.Collections.IEnumerator TeleportSequence()
    {
        // Optional: Disable player controls during teleport
        if (currentPlayer != null)
        {
            currentPlayer.enabled = false;
        }

        // Wait for specified delay
        yield return new WaitForSeconds(teleportDelay);

        // Teleport the player
        if (currentPlayer != null)
        {
            currentPlayer.transform.position = teleportDestination.position;

            // Restore full health
            if (playerHealth != null)
            {
                playerHealth.RestoreFullHealth();
            }
            
            currentPlayer.enabled = true;
        }
    }
}
