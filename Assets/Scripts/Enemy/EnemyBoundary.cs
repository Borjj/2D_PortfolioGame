using UnityEngine;

public class EnemyBoundary : MonoBehaviour
{
    private BoxCollider2D boundaryCollider;
    private Bounds bounds;

    private void Start()
    {
        boundaryCollider = GetComponent<BoxCollider2D>();
        
        if (boundaryCollider != null)
        {
            boundaryCollider.isTrigger = true;
            bounds = boundaryCollider.bounds;
        }
    }

    private void FixedUpdate()
    {
        // Actively check for enemies
        Collider2D[] enemies = Physics2D.OverlapBoxAll(boundaryCollider.bounds.center, boundaryCollider.bounds.size, 0f, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;
            
            // Check if enemy is outside bounds
            if (!bounds.Contains(enemyPos))
            {
                // Clamp position within bounds
                enemyPos.x = Mathf.Clamp(enemyPos.x, bounds.min.x, bounds.max.x);
                enemyPos.y = Mathf.Clamp(enemyPos.y, bounds.min.y, bounds.max.y);
                
                // Move enemy back inside
                enemy.transform.position = enemyPos;

                // If enemy has Rigidbody2D, reset its velocity
                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (boundaryCollider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawCube(boundaryCollider.bounds.center, boundaryCollider.bounds.size);
        }
    }
}