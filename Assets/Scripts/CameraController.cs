using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Ensure the camera position is always on whole pixels
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * 16) / 16f;
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * 16) / 16f;

        transform.position = smoothedPosition;
    }
}