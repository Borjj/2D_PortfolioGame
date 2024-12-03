using UnityEngine;
using System.Collections.Generic;

public class LootBag : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public string itemName;
        public Sprite icon;
        public int quantity;
        public int minQuantity;
        public int maxQuantity;

        [Range(0f, 100f)]
        public float dropChance;
    }

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private GameObject lootWindowPrefab;
    
    [Header("Loot")]
    [SerializeField] private LootItem[] possibleLoot;
    private static bool keyFragmentDropped = false;
    
    private List<LootItem> generatedLoot;
    private GameObject activeWindow;
    private bool isPlayerInRange;
    private MenuManager menuManager;

// -------------------------------------------------------------------------------------- //

    private void Start()
    {
        // Find the interaction prompt in the canvas
        GameObject canvas = GameObject.Find("CanvasMain");
        if (canvas != null)
        {
            interactionPrompt = canvas.transform.Find("InteractionPrompt")?.gameObject;
        }
        if (interactionPrompt == null)
        {
             Debug.LogError("InteractionPrompt not found in Canvas!");
        }

        GenerateLoot();

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        // Find the MenuManager in the scene
        menuManager = FindObjectOfType<MenuManager>();
        if (menuManager == null)
            Debug.LogWarning("MenuManager not found in scene!");
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            ToggleLootWindow();
        }
    }

// -------------------------------------------------------------------------------------- //

    private void GenerateLoot()
    {
        generatedLoot = new List<LootItem>();

        foreach (LootItem item in possibleLoot)
        {
            // Skip key fragment if it's already been dropped
            if (item.itemName.ToLower().Contains("key fragment") && keyFragmentDropped)
                continue;

            if (Random.Range(0f, 100f) <= item.dropChance)
            {
                Debug.Log($"Adding item: {item.itemName}, Icon: {(item.icon != null ? "Present" : "Missing")}, Quantity: {item.quantity}");
                generatedLoot.Add(new LootItem
                {
                    itemName = item.itemName,
                    icon = item.icon,
                    quantity = Random.Range(item.minQuantity, item.maxQuantity + 1),
                    dropChance = item.dropChance
                });

                // Mark key fragment as dropped if this was one
                if (item.itemName.ToLower().Contains("key fragment"))
                    keyFragmentDropped = true;
            }
        }
    }

    private void ToggleLootWindow()
    {
        Debug.Log($"ToggleLootWindow called, items count: {generatedLoot.Count}");
        if (activeWindow == null)
        {
            // Instantiate window at canvas level
            activeWindow = Instantiate(lootWindowPrefab);

            LootWindowUI_Squares windowUI = activeWindow.GetComponent<LootWindowUI_Squares>();
            if (windowUI != null)
            {
                Debug.Log("Found WindowUI, initializing...");
                windowUI.Initialize(generatedLoot, this);
                Debug.Log("Window initialized");
            }
            else
            {
                Debug.LogError("WindowUI component not found!");
            }

            // Show and unlock cursor
            if (menuManager != null)
            {
                menuManager.UnlockCursor();
            }
        }
        else
        {
            CloseWindow();
        }
    }

    public void CloseWindow()
    {
        if (activeWindow != null)
        {
            Destroy(activeWindow);
            activeWindow = null;

            // Hide and lock cursor again
            if (menuManager != null)
            {
                menuManager.LockCursor();
            }
        }
    }

    public void RemoveItem(int index)
    {
        if (index >= 0 && index < generatedLoot.Count)
        {
            generatedLoot.RemoveAt(index);
            
            if (generatedLoot.Count == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public LootItem GetItemAtIndex(int index)
    {
        if (index >= 0 && index < generatedLoot.Count)
        {
            return generatedLoot[index];
        }
        return null;
    }

    public List<LootBag.LootItem> GetItems()
    {
        return generatedLoot;
    }

    public static void ResetKeyFragmentTracking()
    {
        keyFragmentDropped = false;
    }
// ------------------------------------------------------------------------------------ //

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            CloseWindow();
        }
    }

    // Make sure to relock cursor if the loot bag is destroyed while open
    private void OnDestroy()
    {
        if (activeWindow != null)
        {
            CloseWindow();
        }
    }

}