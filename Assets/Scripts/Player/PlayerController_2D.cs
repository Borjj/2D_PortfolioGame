using UnityEngine;
using System.Collections;

public partial class PlayerController_2D : MonoBehaviour
{
    [SerializeField] private float collisionOffset = 0.05f;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Animation Timing")]
    [SerializeField] private float dashAnimationDelay = 0.1f; // Time before movement starts

    [Header("Dash Settings")]
    [SerializeField] private bool dashUnlocked = false;  // Now false by default
    [SerializeField] private GameObject dashVisualIndicator;  // Optional: to show dash is available
    [SerializeField] private CooldownUI dashCooldownUI;
    private Vector2 movement;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Dash Combat")]
    [SerializeField] private float dashDamage = 25f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float damageRadius = 0.5f;
    [SerializeField] private GameObject dashEffectPrefab;

    [Header("Key Fragments")]
    [SerializeField] private GameObject[] fragmentSprites = new GameObject[3];
    private int collectedFragments = 0;

    [Header ("Keys")]
    [SerializeField] private bool hasKey = false;
    [SerializeField] private bool hasBossKey = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;


    public System.Action<int> OnFragmentCollected;
    public System.Action<float> OnFragmentTimerUpdate;

// ---------------------------------------------------------------------------- //

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        dashVisualIndicator.SetActive(false);
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
        // Initialize dash
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        // Start animation before movement
        animator.SetBool("isDashing", true);
        dashCooldownUI.StartCooldown();

        // Play Dash Sound FX
        PlayDashSound();

        // Store dash direction when dash starts
        Vector2 dashDirection = movement.normalized;
        
        // Wait for animation to start
        yield return new WaitForSeconds(dashAnimationDelay);

        // Apply dash movement
        float dashTimer = 0;

        while (dashTimer < dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            dashTimer += Time.deltaTime;
            yield return null;
        }

        // Check for enemies in dash path
        RaycastHit2D[] hits = Physics2D.CircleCastAll
        (
            rb.position,
            damageRadius,
            dashDirection,
            dashSpeed * dashDuration,
            enemyLayer
        );

        foreach (RaycastHit2D hit in hits) 
        {
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(dashDamage);
                if (dashEffectPrefab != null)
                {
                    Instantiate(dashEffectPrefab, hit.point, Quaternion.identity);
                }
            }
        }

        // End dash movement
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("isDashing", false);

        // Cooldown period
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


    private void CollectFragment()
    {
        if (collectedFragments < 3)
        {
            fragmentSprites[collectedFragments].SetActive(true);
            collectedFragments++;
        }

        if (collectedFragments >= 3)
        {
            StartCoroutine(HideFragmentUI());
        }
    }

    private IEnumerator HideFragmentUI()
    {
        yield return new WaitForSeconds(2f);

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