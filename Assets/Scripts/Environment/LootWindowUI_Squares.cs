using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LootWindowUI_Squares : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button closeButton;
    
    [Header("Layout Settings")]
    [SerializeField] private Vector2 slotSize = new Vector2(40f, 40f);  // Square slots
    [SerializeField] private float slotSpacing = 5f;
    [SerializeField] private int slotsPerRow = 4;  // Control horizontal layout

    private LootBag parentBag;
    private List<GameObject> spawnedSlots = new List<GameObject>();

// -------------------------------------------------------------------------------------------- //

    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
        }

        if (closeButton != null)
        {
            Debug.Log("Button found");
            closeButton.onClick.RemoveAllListeners(); // Clear existing listeners
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            Debug.Log("Added click listener");
        }
    }

    private void Start()
    {
        GridLayoutGroup layoutGroup = itemContainer.GetComponent<GridLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = itemContainer.gameObject.AddComponent<GridLayoutGroup>();
        }
        
        // Configure grid layout
        layoutGroup.cellSize = slotSize;
        layoutGroup.spacing = new Vector2(slotSpacing, slotSpacing);
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = slotsPerRow;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter; // center alignment
    }

// -------------------------------------------------------------------------------------------- //

    public void Initialize(List<LootBag.LootItem> items, LootBag bag)
    {
        Debug.Log($"Initialize called with items: {items?.Count}, bag null?: {bag == null}");
        parentBag = bag;

        // Test if parentBag was set
        Debug.Log($"After setting, parentBag null?: {parentBag == null}");
        CreateItemSlots(items);
        
        // Center the window on screen
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
    }

    private void CreateItemSlots(List<LootBag.LootItem> items)
    {
        Debug.Log($"CreateItemSlots: items count = {items?.Count}, itemContainer null? = {itemContainer == null}");
        // Clear existing slots
        foreach (var slot in spawnedSlots)
        {
            if (slot != null)
                Destroy(slot);
        }
        spawnedSlots.Clear();

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("No items to display in loot window");
            return;
        }

        // Create new slots
        foreach (LootBag.LootItem item in items)
        {
            if (itemSlotPrefab == null)
            {
                Debug.LogError("Item slot prefab is not assigned!");
                return;
            }

            Debug.Log($"Creating slot for item: {item.itemName}, Icon present?: {item.icon != null}");
            GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
            spawnedSlots.Add(slot);

            ConfigureSlot(slot, item, spawnedSlots.Count - 1);
        }
    }

    private void ConfigureSlot(GameObject slot, LootBag.LootItem item, int index)
    {
        // Force correct size for slot and its components
        RectTransform slotRect = slot.GetComponent<RectTransform>();

        if (slotRect != null)
        {
            slotRect.sizeDelta = slotSize;
        }

        // Configure components
        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
        TextMeshProUGUI quantity = slot.transform.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        Button button = slot.GetComponent<Button>();

        // Set up button functionality
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => HandleItemClick(index));
        }

        // Configure display elements
        if (icon != null && item.icon != null)
        {
            icon.sprite = item.icon;
            icon.preserveAspect = true;

            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                float iconSize = slotSize.x * 0.8f;
                iconRect.sizeDelta = new Vector2(iconSize, iconSize);
            }
            
        }

        if (quantity != null)
        {
            bool showQuantity = item.quantity > 0;
            quantity.gameObject.SetActive(showQuantity);
            quantity.text = showQuantity ? $"x{item.quantity}" : "";
        }
    }

    private void HandleItemClick(int index)
    {
        if (parentBag != null && index < spawnedSlots.Count)
        {
            LootBag.LootItem item = parentBag.GetItemAtIndex(index);
            
            if (item != null)
            {
                // Handle coin collection
                if (item.itemName.ToLower().Contains("coin"))
                {
                    for (int i = 0; i < item.quantity; i++)
                    {
                        ScoreManager.Instance?.AddCoin();
                    }
                }
                // Handle key fragment collection
                else if (item.itemName.ToLower().Contains("key fragment"))
                {
                    PlayerController_2D player = FindObjectOfType<PlayerController_2D>();
                    if (player != null)
                    {
                        player.CollectFragment();
                    }
                }

                // Remove from data
                parentBag.RemoveItem(index);

                // Update UI
                RefreshSlots(parentBag.GetItems());
            }
        }
    }

    private void RefreshSlots(List<LootBag.LootItem> items)
    {
        // Clear existing slots
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot);
        }
        spawnedSlots.Clear();

        // Create new slots for remaining items
        CreateItemSlots(items);
    }

    private void OnCloseButtonClicked()
    {
        Debug.Log("Close button clicked");
        if (parentBag == null)
        {
            Debug.LogError("ParentBag is null, destroying window directly");
            Destroy(gameObject);
            return;
        }
        parentBag.CloseWindow();
    }

    private void OnEnable()
    {
        Debug.Log("UI Window enabled");
    }
}
