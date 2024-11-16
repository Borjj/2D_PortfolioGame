using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private bool hideCursorOnLock = true;
    [SerializeField] private CursorLockMode defaultLockMode = CursorLockMode.Locked;
    
    private bool isLocked = false;

    private void Start()
    {
        if (lockCursorOnStart)
        {
            LockCursor();
        }
    }

    private void Update()
    {
        // Optional: Add escape key to unlock cursor for testing/debugging
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLocked)
                UnlockCursor();
            else
                LockCursor();
        }
    }

    public void LockCursor()
    {
        isLocked = true;
        // Set the cursor lock mode
        Cursor.lockState = defaultLockMode;
        // Hide or show cursor based on settings
        Cursor.visible = !hideCursorOnLock;
    }

    public void UnlockCursor()
    {
        isLocked = false;
        // Reset cursor state
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Public method to check cursor state
    public bool IsCursorLocked() => isLocked;

    // Clean up when the script is disabled or destroyed
    private void OnDisable()
    {
        UnlockCursor();
    }
}
