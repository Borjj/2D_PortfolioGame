using UnityEngine;
using System.Collections;

public class EnemyController_Wandering : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 0.7f;

    [Header("Movement + Wandering")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] public bool canWander = true;
    [SerializeField] private float wanderRange = 3f;    
    [SerializeField] private float minWanderTime = 1f;  
    [SerializeField] private float maxWanderTime = 3f;  
    [SerializeField] private float timeToWander = 2f;


    private AudioSource movementAudioSource;
    private bool isCurrentlyMoving = false;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyAttack attackComponent;
    
    private Vector2 wanderTarget;
    private float lostPlayerTimer = 0f;
    private bool isWandering = false;
    private bool wasFollowingPlayer = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackComponent = GetComponent<EnemyAttack>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            wasFollowingPlayer = true;
            lostPlayerTimer = 0f;
            isWandering = false;

            if (distanceToPlayer <= attackRange)
            {
                StopMoving();
                if (attackComponent != null && attackComponent.CanAttack())
                {
                    attackComponent.TryAttack();
                }
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
        else
        {
            if (wasFollowingPlayer)
            {
                StopMoving();
                lostPlayerTimer += Time.deltaTime;

                if (canWander && lostPlayerTimer >= timeToWander && !isWandering)
                {
                    wasFollowingPlayer = false;
                    StartCoroutine(Wander());
                }
            }
            else if (canWander && !isWandering)
            {
                StartCoroutine(Wander());
            }
        }

        UpdateFacing();
    }

// ------------------------------------------------------------------------------------------ //

    private IEnumerator Wander()
    {
        isWandering = true;

        while (!wasFollowingPlayer)
        {
            float randomDistance = Random.Range(wanderRange * 0.5f, wanderRange);
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            wanderTarget = (Vector2)transform.position + (randomDirection * randomDistance);
            
            float wanderDuration = Random.Range(minWanderTime, maxWanderTime);
            float elapsedTime = 0f;

            while (elapsedTime < wanderDuration && !wasFollowingPlayer)
            {
                Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
                
                if (Vector2.Distance(transform.position, wanderTarget) > 0.1f)
                {
                    rb.velocity = direction * moveSpeed * 0.5f;
                    animator.SetBool("isMoving", true);
                    UpdateMovementSound(true);
                }
                else
                {
                    break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            StopMoving();
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }

        isWandering = false;
    }
    private void UpdateMovementSound(bool isMoving)
    {
        if (isMoving != isCurrentlyMoving)
        {
            isCurrentlyMoving = isMoving;
            if (isMoving)
            {
                // Start looped sound
                if (movementAudioSource == null || !movementAudioSource.isPlaying)
                {
                    movementAudioSource = AudioManager.Instance?.PlayLoopedSound("EnemyMove", transform.position);
                }
            }
            else
            {
                // Stop sound
                AudioManager.Instance?.StopSound(movementAudioSource);
            }
        }

        // Update position of sound while moving
        if (isMoving && movementAudioSource != null)
        {
            movementAudioSource.transform.position = transform.position;
        }
    }

    // Add this to ensure clean up when enemy is destroyed
    private void OnDestroy()
    {
        AudioManager.Instance?.StopSound(movementAudioSource);
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("isMoving", true);
        UpdateMovementSound(true);
    }

    private void StopMoving()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        UpdateMovementSound(false);
    }

    private void UpdateFacing()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.velocity.x < 0;
        }
    }

// ------------------------------------------------------------------------------- //
    
    private void OnDrawGizmosSelected()
    {
        // Detection and attack ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        
        if (canWander)
        {
            // Wander range
            Gizmos.color = new Color(0, 1, 1, 0.3f); // Semi-transparent Cyan.
            Gizmos.DrawSphere(transform.position, wanderRange);
            
            // Show where the enemy will wander
            if (isWandering && Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(wanderTarget, 0.2f);
                Gizmos.DrawLine(transform.position, wanderTarget);
            }
        }
    }
}