using UnityEngine;

public class AdvancedCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Following")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Zoom")]
    public float minZoom = 2f;
    public float maxZoom = 10f;
    [Range(2f, 10f)]
    public float currentZoom = 5f;

    private Camera cam;


// ------------------------------------------------------------------------------------------- //


    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to the camera. Please assign a target in the inspector.");
        }

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        // Initialize zoom
        UpdateZoom();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Ensure the camera position is always on whole pixels for crisp pixel art
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * (100f/3f)) / (100f/3f);
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * (100f/3f)) / (100f/3f);
        // Maintain the z-position from the offset
        smoothedPosition.z = offset.z;

        // Set the camera's position
        transform.position = smoothedPosition;

        // Update zoom
        UpdateZoom();

        // Debug log
        //Debug.Log($"Camera pos: {transform.position}, Target pos: {target.position}, Desired pos: {desiredPosition}");
    }


// -------------------------------------------------------------------------------------------- //

    private void UpdateZoom()
    {
        cam.orthographicSize = currentZoom;
    }

    // Public method to adjust zoom programmatically if needed
    public void SetZoom(float newZoom)
    {
        currentZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
    }
}