using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float currentHealth;
    
    [Header("UI")]
    [SerializeField] private Image healthBarFill;
    
    [Header("Respawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private int currentSpawnPointIndex = 0;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashRate = 0.1f;    // How fast the sprite flashes
    [SerializeField] private Color damageFlashColor = new Color(1, 0, 0, 0.5f);  // Red flash

    // Component references
    private Animator animator;
    private PlayerController_2D playerController;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // State
    private bool isDead = false;
    private bool isInvincible = false;
    private Color originalColor;


// ---------------------------------------------------------------------------------- //

    private void Start()
    {
        // Get components
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController_2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store original sprite color
        originalColor = spriteRenderer.color;

        // Initialize health
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

// ---------------------------------------------------------------------------------- //


    public void TakeDamage(float damage)
    {
        // If dead or invincible, don't take damage
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsedTime = 0f;
        bool isFlashing = false;

        // Flash the sprite while invincible
        while (elapsedTime < invincibilityDuration)
        {
            // Toggle between original and flash color
            spriteRenderer.color = isFlashing ? damageFlashColor : originalColor;
            isFlashing = !isFlashing;
            
            yield return new WaitForSeconds(flashRate);
            elapsedTime += flashRate;
        }

        // Ensure sprite returns to original color
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;

        // Stop invincibility coroutine if running
        StopAllCoroutines();
        
        // Reset sprite color
        spriteRenderer.color = originalColor;

        // Trigger death animation
        animator.SetBool("isDead", true);

        // Disable player control
        DisablePlayer();

        // Start respawn sequence
        Invoke("Respawn", respawnDelay);
    }

    private void DisablePlayer()
    {
        // Disable movement and physics
        playerController.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

    private void EnablePlayer()
    {
        // Re-enable movement and physics
        playerController.enabled = true;
        rb.isKinematic = false;
    }

    private void Respawn()
    {
        // Reset health
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Reset position
        if (spawnPoints != null && spawnPoints.Length > 0 && currentSpawnPointIndex < spawnPoints.Length)
        {
            transform.position = spawnPoints[currentSpawnPointIndex].position;
        }

        // Reset animation
        animator.SetBool("isDead", false);

        // Re-enable player
        EnablePlayer();

        isDead = false;
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }   

    // Call this when player reaches a new checkpoint/level
    public void SetSpawnPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            currentSpawnPointIndex = index;
        }
    }
}