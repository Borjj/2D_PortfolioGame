using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        // Add hit feedback (animation, particles, sound)
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death effects
        Destroy(gameObject);
    }
}
