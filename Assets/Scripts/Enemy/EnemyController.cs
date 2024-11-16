using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    // References
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyAttack attackComponent;

    [SerializeField] private bool debugMode = true;

// ----------------------------------------------------------------------------------- //

    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackComponent = GetComponent<EnemyAttack>();
        
        // Find player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (attackComponent == null)
        {
            Debug.LogError("EnemyAttack component is missing!");
        }
        if (animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            // If in attack range, stop and attack
            if (distanceToPlayer <= attackRange)
            {
                StopMoving();
                if (debugMode) Debug.Log("In attack range, distance: " + distanceToPlayer);

                if (attackComponent != null)
                {
                    if (attackComponent.CanAttack())
                    {
                        if (debugMode) Debug.Log("Can Attack: true");
                        attackComponent.TryAttack();
                    }
                    else
                    {
                        if (debugMode) Debug.Log("Can Attack: false");
                    }
                }
                else
                {
                    Debug.LogError("Attack component is null!");
                }
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
        else
        {
            StopMoving();
        }

        // Update facing direction
        UpdateFacing();
    }

// ----------------------------------------------------------------------------------- //


    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("isMoving", true);
    }

    private void StopMoving()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    private void UpdateFacing()
    {
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}