using UnityEngine;

public class DashPowerUp : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] private float hoverAmount = 0.07f;
    [SerializeField] private float hoverSpeed = 1.5f;
    
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem collectEffect;
        
    private Vector3 startPosition;


// ------------------------------------------------------------------------------------------------ //


    private void Start()
    {
        startPosition = transform.position;

        // Disable particle system at start
        if (collectEffect != null)
            collectEffect.gameObject.SetActive(false);
    }

    private void Update()
    {
        Movement();
    }


// ------------------------------------------------------------------------------------------- //


    private void Movement()
    {
        // Rotate the power-up
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        
        // Add hovering effect
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController_2D player = other.GetComponent<PlayerController_2D>();
            if (player != null)
            {
                // Play collection effect
                if (collectEffect != null)
                {
                    collectEffect.gameObject.SetActive(true);
                    collectEffect.Play();
                    PlayPowerUpSound();
                }

                player.UnlockDash();
                GetComponent<SpriteRenderer>().enabled = false;

                // Destroy after particle effect finishes
                float destroyDelay = collectEffect != null ? collectEffect.main.duration : 0f;
                Destroy(gameObject, destroyDelay);
            }
        }
    }

    private void PlayPowerUpSound()
    {
        AudioManager.Instance?.PlaySound("PowerUp");
    }
}