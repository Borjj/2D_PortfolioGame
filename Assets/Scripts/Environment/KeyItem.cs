using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [SerializeField] private GameObject keyUIImage; // Reference to UI image showing the key
        [SerializeField] private GameObject keyUIImageBOSS; // Reference to UI image showing the key

    [SerializeField] private bool isBossKey = false;
    [SerializeField] private GameObject collectEffect; // Optional particle effect

    [Header("Movement Settings")]
    [SerializeField] private float hoverAmount = 0.07f;
    [SerializeField] private float hoverSpeed = 1.5f;

    private Vector3 startPosition;
    private GameObject canvas;
    
// -------------------------------------------------------------------------------------------- //

    private void Start()
    {
        canvas = GameObject.Find("CanvasMain");
        keyUIImage = canvas.transform.Find("Key_UI")?.gameObject;
        keyUIImageBOSS = canvas.transform.Find("Boss_Key_UI")?.gameObject;

        startPosition = transform.position;
        keyUIImage.SetActive(false);
        keyUIImageBOSS.SetActive(false);
    }

    private void Update()
    {
        Movement();
        CheckTag();
    }

// -------------------------------------------------------------------------------------------- //

    private void Movement()
    {        
        // Add hovering effect
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void CheckTag()
    {
        if(tag == "BossKey")
        {
            isBossKey = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Show key in UI
            if (keyUIImage != null || keyUIImageBOSS != null)
            {
                keyUIImage.SetActive(true);
                keyUIImageBOSS.SetActive(true);
            }

            // Play effects if assigned
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }
            
            // Give key to player
            if (isBossKey)
            {
                other.GetComponent<PlayerController_2D>().CollectBossKey();
                PlayBossKeySound();
            }
            else
            {
                other.GetComponent<PlayerController_2D>().CollectKey();
                PlayKeySound();
            }
            
            // Destroy key object
            Destroy(gameObject);
        }
    }

    private void PlayKeySound()
    {
        AudioManager.Instance?.PlaySound("Key");
    }
    private void PlayBossKeySound()
    {
        AudioManager.Instance?.PlaySound("BossKey");
    }
}
