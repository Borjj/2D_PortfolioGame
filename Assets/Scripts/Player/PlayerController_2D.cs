using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class PlayerController_2D : MonoBehaviour
{
    [SerializeField] private float collisionOffset = 0.05f;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 movement;

    [Header("Animation Timing")]
    [SerializeField] private float dashAnimationDelay = 0.1f; // Time before movement starts

    [Header("Dash Settings")]
    [SerializeField] private bool dashUnlocked = false;  // Now false by default
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private GameObject dashVisualIndicator;  // Optional: to show dash is available
    [SerializeField] private CooldownUI dashCooldownUI;
    
    private bool canDash = true;
    public bool isDashing = false;
    private int originalLayer;
    private ContactFilter2D enemyFilter;

    [Header("Dash Combat")]
    [SerializeField] private float dashDamage = 25f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float damageRadius = 0.5f;
    [SerializeField] private GameObject dashHitPrefab;

    [Header("Key Fragments")]
    [SerializeField] private GameObject[] fragmentSprites = new GameObject[3];
    private int collectedFragments = 0;

    [Header ("Keys")]
    [SerializeField] private bool hasKey = false;
    [SerializeField] private bool hasBossKey = false;


    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerHealth playerHealth;


    public System.Action<int> OnFragmentCollected;
    public System.Action<float> OnFragmentTimerUpdate;

// ---------------------------------------------------------------------------- //

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        dashVisualIndicator.SetActive(false);

        // Store original layer and setup enemy filter
        originalLayer = gameObject.layer;
        enemyFilter = new ContactFilter2D();
        enemyFilter.SetLayerMask(enemyLayer);
    }

    private void Start()
    {
        foreach (GameObject sprite in fragmentSprites)
        {
            sprite.SetActive(false);
        }
    }

    private void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Dash input
        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing && dashUnlocked)
        {
            StartCoroutine(Dash());
        }

        // Flip sprite
        if (movement.x != 0)
        {
            spriteRenderer.flipX = movement.x < 0;
        }

        // Update Animator
        UpdateAnimator();

    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }
    }

// ---------------------------------------------------------------------------------------------- //

    private void Move()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    public void UnlockDash()
    {
        dashUnlocked = true;

        if (dashVisualIndicator != null)
        {
            dashVisualIndicator.SetActive(true);
        }
        // Optional: Add visual/audio feedback when dash is unlocked
        dashVisualIndicator.SetActive(true);
    }

    // Optional: Add this method if you want to check dash status
    public bool IsDashUnlocked()
    {
        return dashUnlocked;
    }

    private IEnumerator Dash()
    {
        if (isDashing) yield break;  // Prevent dash overlap

        // Initialize dash
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        // Store dash direction when dash starts
        Vector2 dashDirection = movement.normalized;

        // Make player invulnerable while Dashing
        if (playerHealth != null)
        {
            playerHealth.SetInvulnerability(true);
        }

        // Change layer to ignore enemies temporarily
        gameObject.layer = LayerMask.NameToLayer("DashingPlayer");
        
        // Start animation and UI before movement
        animator.SetBool("isDashing", true);
        dashCooldownUI.StartCooldown();
        PlayDashSound();

        // Animation startup delay
        yield return new WaitForSeconds(dashAnimationDelay);

        // Track enemies hit to prevent multiple damage instances
        HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();
        List<Collider2D> enemiesInRange = new List<Collider2D>();

        // Dash movement phase
        float dashTimer = 0;
        while (dashTimer < dashDuration)
        {
            if (isDashing)
            {
                rb.velocity = dashDirection * dashSpeed;
                
                // Check for enemies along the dash path
                Physics2D.OverlapCircle(rb.position, damageRadius, enemyFilter, enemiesInRange);
                foreach (Collider2D enemy in enemiesInRange)
                {
                    if (!hitEnemies.Contains(enemy))
                    {
                        if (enemy.TryGetComponent<EnemyHealth>(out var enemyHealth))
                        {
                            enemyHealth.TakeDamage(dashDamage);
                            if (dashHitPrefab != null)
                            {
                                Instantiate(dashHitPrefab, enemy.transform.position, Quaternion.identity);
                            }
                            hitEnemies.Add(enemy);
                        }
                    }
                }
            }
            dashTimer += Time.deltaTime;
            yield return null;
        }

        // Handle enemy damage
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            rb.position,
            damageRadius,
            dashDirection,
            dashSpeed * dashDuration,
            enemyLayer
        );

        foreach (RaycastHit2D hit in hits) 
        {
            if (hit.collider.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(dashDamage);
                if (dashHitPrefab != null)
                {
                    Instantiate(dashHitPrefab, hit.point, Quaternion.identity);
                }
            }
        }

        // Clean ending - ensure everything resets properly
        gameObject.layer = originalLayer;
        isDashing = false;
        rb.gravityScale = originalGravity;
        rb.velocity = Vector2.zero;
        animator.SetBool("isDashing", false);

        // Wait for cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void PlayDashSound()
    {
        AudioManager.Instance?.PlaySound("Dash");
    }

    private void UpdateAnimator()
    {
        bool isMoving = movement.magnitude > 0.1f;
        animator.SetBool ("isMoving", isMoving);
    }

// ------------------------------------------------------------------ //

    // Key States
    public void CollectKey()
    {
        hasKey = true;
    }
    public void CollectBossKey()
    {
        hasBossKey = true;
    }

    public void UseKey()
    {
        hasKey = false;
    }

    public void UseBossKey()
    {
        hasBossKey = false;
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public bool HasBossKey()
    {
        return hasBossKey;
    }


    public void CollectFragment()
    {
        if (collectedFragments < 3)
        {
            fragmentSprites[collectedFragments].SetActive(true);
            collectedFragments++;
        }

    }
    public bool HasAllKeyFragments()
    {
        return collectedFragments >= 3;
    }

    public void ConsumeKeyFragments()
    {
        foreach (GameObject sprite in fragmentSprites)
        {
            sprite.SetActive(false);
        }
        collectedFragments = 0;
    }
    
    // ---------------------------------------------------------------------------------------- //

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("KeyFragment"))
            {
                CollectFragment();
                Destroy(other.gameObject);
            }
        }

    private void OnDrawGizmos()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rb.position, rb.position + movement.normalized);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(rb.position, moveSpeed * Time.fixedDeltaTime + collisionOffset);
        }
    }
}