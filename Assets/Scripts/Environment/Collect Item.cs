using UnityEngine;

public partial class CollectItem : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private GameObject collectEffect; // Optional particle effect
    [SerializeField] private AudioClip collectSound;   // Optional sound effect

    [Header("Type of Item")]
    [SerializeField] private bool key;
    [SerializeField] private bool bossKey;
    [SerializeField] private bool keyFragment;
    [SerializeField] private bool coin;
    
// --------------------------------------------------------------------------------------- //

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add coin to score
            if(coin)
            {
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddCoin();
                }
            }

            // Play effects if assigned
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

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

            // Destroy the coin
            Destroy(gameObject);
        }
    }

    private void PlayCollectSound()
    {
        AudioManager.Instance?.PlaySound("Collect");
    }
}