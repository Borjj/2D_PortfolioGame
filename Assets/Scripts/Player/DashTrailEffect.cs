using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class DashTrailEffect : MonoBehaviour
{
    [Header("Trail Settings")]
    [SerializeField] private float trailLifetime = 0.5f;     
    [SerializeField] private float spawnInterval = 0.05f;    
    [SerializeField] private Color trailStartColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color trailEndColor = new Color(1f, 1f, 1f, 0f);
    
    [Header("References")]
    [SerializeField] private SpriteRenderer playerSprite;    
    
    private float nextSpawnTime;
    private List<TrailPiece> trailPieces = new List<TrailPiece>();
    private PlayerController_2D playerController;
    private FieldInfo isDashingField;
    
    private class TrailPiece
    {
        public GameObject gameObject;
        public SpriteRenderer spriteRenderer;
        public float spawnTime;
    }

    private void Start()
    {
        // Get required components
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
            if (playerSprite == null)
            {
                //Debug.LogError("No SpriteRenderer found for trail effect!");
                enabled = false;
                return;
            }
        }

        playerController = GetComponent<PlayerController_2D>();
        if (playerController == null)
        {
            //Debug.LogError("PlayerController_2D not found!");
            enabled = false;
            return;
        }

        // Set up reflection to access private isDashing field
        isDashingField = typeof(PlayerController_2D).GetField("isDashing", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (isDashingField == null)
        {
            //Debug.LogError("Could not find isDashing field in PlayerController_2D!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (isDashingField == null || playerController == null) return;

        bool isDashing = (bool)isDashingField.GetValue(playerController);

        if (isDashing && Time.time >= nextSpawnTime)
        {
            SpawnTrailPiece();
            nextSpawnTime = Time.time + spawnInterval;
        }

        UpdateTrailPieces();
    }

    private void SpawnTrailPiece()
    {
        GameObject trailObject = new GameObject("TrailPiece");
        trailObject.transform.position = transform.position;
        trailObject.transform.rotation = transform.rotation;
        trailObject.transform.localScale = transform.localScale;

        SpriteRenderer trailRenderer = trailObject.AddComponent<SpriteRenderer>();
        trailRenderer.sprite = playerSprite.sprite;
        trailRenderer.sortingOrder = playerSprite.sortingOrder - 1;
        trailRenderer.flipX = playerSprite.flipX;
        trailRenderer.color = trailStartColor;

        TrailPiece trailPiece = new TrailPiece
        {
            gameObject = trailObject,
            spriteRenderer = trailRenderer,
            spawnTime = Time.time
        };
        trailPieces.Add(trailPiece);
    }

    private void UpdateTrailPieces()
    {
        for (int i = trailPieces.Count - 1; i >= 0; i--)
        {
            TrailPiece trail = trailPieces[i];
            float age = Time.time - trail.spawnTime;
            
            if (age > trailLifetime)
            {
                Destroy(trail.gameObject);
                trailPieces.RemoveAt(i);
            }
            else
            {
                float lifePercent = age / trailLifetime;
                trail.spriteRenderer.color = Color.Lerp(trailStartColor, trailEndColor, lifePercent);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var trail in trailPieces)
        {
            if (trail.gameObject != null)
            {
                Destroy(trail.gameObject);
            }
        }
        trailPieces.Clear();
    }
}