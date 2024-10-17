using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private ContactFilter2D movementFilter;
    [SerializeField] private float collisionOffset = 0.05f;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();


// ------------------------------------------------------------------------------------------- //

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }

            animator.SetBool("isMoving", success);
            if (movementInput.x != 0)
            {
                spriteRenderer.flipX = (movementInput.x < 0);
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        animator.SetFloat("moveX", Mathf.Abs(movementInput.x));
        animator.SetFloat("moveY", movementInput.y);
    }


// ------------------------------------------------------------------------------------- //


    private bool TryMove(Vector2 direction)
    {
        int count = rb.Cast(
            direction,
            movementFilter,
            castCollisions,
            moveSpeed * Time.fixedDeltaTime + collisionOffset
        );

        if (count == 0)
        {
            Vector2 moveVector = direction * moveSpeed * Time.fixedDeltaTime;
            
            // Round the movement to ensure pixel-perfect positioning
            Vector2 newPosition = rb.position + moveVector;
            newPosition.x = Mathf.Round(newPosition.x * 16) / 16f;
            newPosition.y = Mathf.Round(newPosition.y * 16) / 16f;

            rb.MovePosition(newPosition);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }


    // Debug: Draw player's movement direction and collision checks
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