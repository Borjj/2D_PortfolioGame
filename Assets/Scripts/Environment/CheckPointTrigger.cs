using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private int checkpointIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth healthManager = other.GetComponent<PlayerHealth>();
            if (healthManager != null)
            {
                healthManager.SetSpawnPoint(checkpointIndex);
            }
        }
    }
}