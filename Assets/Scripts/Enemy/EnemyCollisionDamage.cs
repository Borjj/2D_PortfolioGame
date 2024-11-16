using UnityEngine;

public class EnemyCollisionDamage : MonoBehaviour
{
    [Header("Collision Damage")]
    [SerializeField] private float collisionDamage = 10f;
    [SerializeField] private float damageRate = 0.5f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.25f;

    [Header("Detection")]
    [SerializeField] private LayerMask enemyLayer;
    
    private float nextDamageTime = 0f;

// -------------------------------------------------------------------------------- //

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (playerHealth != null && playerRb != null)
                {
                    // Deal damage
                    playerHealth.TakeDamage(collisionDamage);
                    
                    // Apply knockback
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    StartCoroutine(ApplyKnockback(playerRb, knockbackDirection));
                    
                    nextDamageTime = Time.time + damageRate;
                }
            }
        }
    }

    private System.Collections.IEnumerator ApplyKnockback(Rigidbody2D rb, Vector2 direction)
    {
        // Store original velocity
        Vector2 originalVelocity = rb.velocity;
        
        // Apply knockback force
        rb.velocity = direction * knockbackForce;
        
        // Wait for knockback duration
        yield return new WaitForSeconds(knockbackDuration);
        
        // Reset to original velocity
        rb.velocity = originalVelocity;
    }

}