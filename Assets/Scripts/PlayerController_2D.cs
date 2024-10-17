using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController_2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float collisionOffset = 0.05f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 currentVelocity;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isDashing;
    private float dashTimeLeft;
    private float lastDashTime;

    // Define animator parameter hashes
    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");
    private static readonly int IsDashingParam = Animator.StringToHash("IsDashing");


// ---------------------------------------------------------------------------------------- //


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Debug.Log("PlayerController_2D Awake");
    }

    private void FixedUpdate()
    {
        Move();
        FlipSprite();
        UpdateAnimations();
    }


// ---------------------------------------------------------------------------------------- //


    private void Move()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                Debug.Log("Dash ended");
            }
        }

        Vector2 targetVelocity = isDashing ? 
            currentVelocity : 
            movementInput * moveSpeed;

        float currentSpeed = isDashing ? dashSpeed : moveSpeed;
        float currentAcceleration = isDashing ? 
            acceleration * 2 : 
            (movementInput.magnitude > 0 ? acceleration : deceleration);

        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, 
            currentAcceleration * Time.fixedDeltaTime);

        rb.velocity = currentVelocity.normalized * currentSpeed;
    }

    private void FlipSprite()
    {
        if (movementInput.x != 0)
        {
            spriteRenderer.flipX = (movementInput.x < 0);
        }
    }

    private void UpdateAnimations()
    {
        bool isMoving = movementInput.magnitude > 0;
        animator.SetBool(IsMovingParam, isMoving);
        animator.SetBool(IsDashingParam, isDashing);
        
        Debug.Log($"UpdateAnimations - IsMoving: {isMoving}, IsDashing: {isDashing}");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log($"OnDash called. Performed: {context.performed}, Time check: {Time.time >= lastDashTime + dashCooldown}");
        if (context.performed && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;
        Debug.Log("Dash started");

        // If not moving, dash in the direction the player is facing
        if (movementInput.magnitude == 0)
        {
            currentVelocity = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }
        else
        {
            currentVelocity = movementInput.normalized;
        }
    }

// ---------------------------------------------------------------------------------------- //

    private void OnDrawGizmos()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rb.position, rb.position + movementInput.normalized);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(rb.position, moveSpeed * Time.fixedDeltaTime + collisionOffset);
        }
    }
}