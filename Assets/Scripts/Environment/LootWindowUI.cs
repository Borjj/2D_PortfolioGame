using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LootWindowUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button closeButton;
    
    [Header("Layout Settings")]
    [SerializeField] private Vector2 slotSize = new Vector2(150f, 21.5f);
    [SerializeField] private float slotSpacing = 5f;

    private LootBag parentBag;
    private List<GameObject> spawnedSlots = new List<GameObject>();

// ------------------------------------------------------------------------------------------ //

    private void Awake()
    {
        // Ensure the canvas is set up correctly
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10; // Ensure it appears above other UI
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void Start()
    {
        VerticalLayoutGroup layoutGroup = itemContainer.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = itemContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        }
        
        // Configure layout
        layoutGroup.spacing = 25f;
        layoutGroup.padding = new RectOffset(5, 5, 5, 5);
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandHeight = false;
    }

// ------------------------------------------------------------------------------------------ //
    public void Initialize(List<LootBag.LootItem> items, LootBag bag)
    {
        parentBag = bag;
        CreateItemSlots(items);
        
        // Center the window on screen
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
    }

    private void CreateItemSlots(List < LootBag.LootItem > items)
    {
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
        for (int i = 0; i < items.Count; i++)
        {
            if (itemSlotPrefab == null)
            {
                Debug.LogError("Item slot prefab is not assigned!");
                return;
            }

            GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
            spawnedSlots.Add(slot);

            ConfigureSlot(slot, items[i], i);
        }

        UpdateSlotPositions();
    }

    private void ConfigureSlot(GameObject slot, LootBag.LootItem item, int index)
    {
        // Force correct size
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        if (slotRect != null)
        {
            slotRect.sizeDelta = slotSize;
            Debug.Log($"Setting slot size to: {slotSize}");
        }

        // Get components
        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
        TextMeshProUGUI itemName = slot.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI quantity = slot.transform.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        Button button = slot.GetComponent<Button>();

        Debug.Log($"Slot {index} components - Icon: {(icon != null)}, ItemName: {(itemName != null)}, Quantity: {(quantity != null)}");
        Debug.Log($"Setting content - Name: {item.itemName}, Icon: {(item.icon != null)}, Quantity: {item.quantity}");

        // Configure button
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                Debug.Log($"Attempting to collect item: {item.itemName} at index {index}");
                HandleItemClick(index);
            });
        }

        // Configure Icon, name and quantity
        if (icon != null && item.icon != null)
        {
            icon.sprite = item.icon;
            icon.color = Color.white;
        }

        if (itemName != null)
        {
            itemName.text = item.itemName;
            itemName.color = Color.white;
        }

        if (quantity != null)
        {
            quantity.text = item.quantity > 0 ? $"x{item.quantity}" : "x1";
            quantity.color = Color.white;
        }

        // Debug log button setup
        Debug.Log($"Button configured for slot {index}: {(button != null ? "Success" : "Failed")}");
    }

    
    private void UpdateSlotPositions()
    {
        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            RectTransform rectTransform = spawnedSlots[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float yPosition = -i * (slotSize.y + slotSpacing);
                rectTransform.anchoredPosition = new Vector2(0, yPosition);
            }
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
        parentBag.CloseWindow();
    }
}