using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    
    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private float flashRate = 0.1f;
    
    private float currentHealth;
    private EnemyLoot loot;
    private float invulnerabilityTimer = 0f;
    private bool isInvulnerable = false;
    
    // Visual feedback
    private SpriteRenderer spriteRenderer;
    private float flashTimer;
    private bool isFlashing;

// -------------------------------------------------------------------------------------------- //

    private void Start()
    {
        currentHealth = maxHealth;
        loot = GetComponent<EnemyLoot>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isInvulnerable)
        {
            // Count down invulnerability
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0)
            {
                isInvulnerable = false;
                spriteRenderer.enabled = true; // Ensure sprite is visible when no longer invulnerable
            }
            
            // Handle flashing
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                flashTimer = flashRate;
                isFlashing = !isFlashing;
                spriteRenderer.enabled = isFlashing;
            }
        }
    }

// -------------------------------------------------------------------------------------------- //

    public void TakeDamage(float damage)
    {
        // If invulnerable, ignore damage
        if (isInvulnerable) return;

        currentHealth -= damage;
        
        // Start invulnerability and flashing
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        flashTimer = flashRate;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        loot?.DropLoot();
        Destroy(gameObject);
    }
}