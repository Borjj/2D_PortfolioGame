using UnityEngine;

public partial class CollectItem : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem collectEffectPrefab; // Optional particle effect
    [SerializeField] private AudioClip collectSound;   // Optional sound effect

    [Header("Type of Item")]
    [SerializeField] private bool key;
    [SerializeField] private bool bossKey;
    [SerializeField] private bool keyFragment;
    [SerializeField] private bool coin;

    [Header("Movement Settings")]
    [SerializeField] private float hoverAmount = 0.07f;
    [SerializeField] private float hoverSpeed = 1.5f;

    private Vector3 startPosition;

    
// --------------------------------------------------------------------------------------- //
    private void Start()
    {
        startPosition = transform.position;        
    }

    private void Update()
    {
        Movement();
    }

// ----------------------------------------------------------------------------------------- //

    private void Movement()
    {        
        // Add hovering effect
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            // Disable renderer and collider but keep object alive for effects
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;


            // Add coin to score
            if(coin && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddCoin();
            }

            // Handle particle effects
            PlayCollectEffect();

            if (collectSound != null)
            {
                if (keyFragment)
                {
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);
                }
                else
                {
                    PlayCollectSound();
                }
            }

            // Destroy the object after effects finish
            float maxDelay = collectEffectPrefab != null ? 
                collectEffectPrefab.GetComponent<ParticleSystem>()?.main.duration ?? 0.5f : 0.1f;
            Destroy(gameObject, maxDelay);
        }
    }

    private void PlayCollectSound()
    {
        AudioManager.Instance?.PlaySound("Collect");
    }

    private void PlayCollectEffect()
    {
        if (collectEffectPrefab != null)
        {
            // Instantiate the prefab particle system
            ParticleSystem particles = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            particles.Play();
            
            // Automatically destroy the particle system after it's done
            float duration = particles.main.duration + particles.main.startLifetime.constantMax;
            Destroy(particles.gameObject, duration);
        }
    }
}