using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Animation Timing")]
    [SerializeField] private float attackDuration = 1f;   // Total duration of attack animation

    // References
    private Animator animator;
    private bool canAttack = true;
    private bool isAttacking = false;

    [SerializeField] private bool debugMode = true;

// ------------------------------------------------------------------------------------- //

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
            return;
        }

        // Verify animator parameters exist
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (debugMode) Debug.Log($"Found parameter: {param.name} of type {param.type}");
        }
    }

// ------------------------------------------------------------------------------------- //

    public bool CanAttack()
    {
        if (debugMode)
        {
            Debug.Log($"CanAttack check - canAttack: {canAttack}, isAttacking: {isAttacking}");
        }

        return canAttack && !isAttacking;
    }

    public void TryAttack()
    {
        if (debugMode) Debug.Log("TryAttack called");
        
        if (CanAttack())
        {
            if (debugMode) Debug.Log("Starting attack sequence");
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
         // Start attack
        isAttacking = true;
        canAttack = false;
        animator.SetBool("isAttacking", true);

        // Wait for attack animation to complete
        yield return new WaitForSeconds(attackDuration);
        
        // End attack
        animator.SetBool("isAttacking", false);
        isAttacking = false;

        // Start cooldown
        yield return new WaitForSeconds(attackCooldown);
        
        // Reset attack availability
        canAttack = true;
    }

    private void CheckDamage()
    {
        if (debugMode) Debug.Log("CheckDamage called");

        // Check for player in attack radius
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}