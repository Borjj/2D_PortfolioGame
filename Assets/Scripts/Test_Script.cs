using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 movement;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on this GameObject");
        }
    }

    private void Update()
    {
        // Get input
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // Calculate movement vector
        movement = new Vector3(moveHorizontal, moveVertical, 0f).normalized;

        // Move the player
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Flip sprite if moving left
        if (moveHorizontal != 0)
        {
            spriteRenderer.flipX = (moveHorizontal < 0);
        }
    }
}