using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Header("Loot Settings")]
    [SerializeField] private GameObject[] possibleLoot;  // Array of items that can be dropped
    [SerializeField] private bool guaranteedDrop = true; // If false, might not drop anything
    [SerializeField] private float dropChance = 50f;     // Chance to drop if not guaranteed (0-100)
    
    public void DropLoot()
    {
        if (possibleLoot == null || possibleLoot.Length == 0) return;
        if (!guaranteedDrop && Random.Range(0f, 100f) > dropChance) return;

        GameObject lootToSpawn = possibleLoot[Random.Range(0, possibleLoot.Length)];
        
        // Add random offset to spawn position
        Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, 0.5f + randomOffset.y, 0);
        
        Instantiate(lootToSpawn, spawnPosition, Quaternion.identity);
    }
}