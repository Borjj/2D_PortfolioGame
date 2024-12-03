using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public Sprite icon;
    public int price;
    public int maxQuantity;  // Maximum units available
    public int stock; // Current available units
    [TextArea] public string description;
}

public class ShopManager : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private GameObject shopWindowPrefab;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject interactionPrompt;

    private GameObject shopWindow;
    private Transform itemContainer;
    private TextMeshProUGUI playerCoinsText;
    
    [Header("Shop Settings")]
    [SerializeField] private ShopItem[] availableItems;
    
    private bool isShopOpen = false;
    private PlayerInventory playerInventory;
    private MenuManager menuManager;
    private bool playerInRange = false;
    private GameObject canvas;

    [Header("UI Feedback")]
    [SerializeField] private GameObject notEnoughCoinsMessage;
    [SerializeField] private float messageDisplayTime = 2f; // How long to show the message

// ----------------------------------------------------------------------------- //
    private void Awake() 
    {
        
    }
    private void Start()
    {
        // Find the interaction prompt in the canvas
        canvas = GameObject.Find("CanvasMain");
        if (canvas != null)
        {
            interactionPrompt = canvas.transform.Find("InteractionPrompt")?.gameObject;
        }

        menuManager = FindObjectOfType<MenuManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        InitializeShopWindow();
        
        foreach (ShopItem item in availableItems)
        {
            item.stock = item.maxQuantity;
        }
        
        UpdateCoinsDisplay();

        if(notEnoughCoinsMessage != null)
        {
            notEnoughCoinsMessage.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerInRange)
        {
            ToggleShop();
        }
    }

// ----------------------------------------------------------------------------- //
    private void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        Debug.Log($"isShopOpen = {isShopOpen}");
        shopWindow.SetActive(isShopOpen);
        
        if (isShopOpen)
        {
            menuManager.UnlockCursor();
            PopulateShop();
        }
        else
        {
            menuManager.LockCursor();
        }
    }
    private void InitializeShopWindow()
    {
        shopWindow = Instantiate(shopWindowPrefab);
        
        // Get references
        itemContainer = shopWindow.transform.Find("BG/ItemContainer");
        if(itemContainer == null)
        {
            Debug.Log("ItemContainer not found");
        }

        playerCoinsText = canvas.transform.Find("Coin/CoinCount").GetComponent<TextMeshProUGUI>(); 

        shopWindow.SetActive(false);     
    }

    private void PopulateShop()
    {
        Debug.Log($"PopulateShop called - itemContainer: {itemContainer != null}, items count: {availableItems.Length}");

        if (itemContainer == null)
        {
            Debug.LogError("item container is null");
            return;
        }

        // Clear existing items
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        // Create slots for each item
        foreach (ShopItem item in availableItems)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
            Debug.Log($"Creating slot for item: {item.itemName}");
            
            // Configure UI elements
            slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;
            slot.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"{item.price}";
            slot.transform.Find("Stock").GetComponent<TextMeshProUGUI>().text = $"{item.stock}";
            slot.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
            
            // Add purchase functionality
            Button buyButton = slot.GetComponent<Button>();
            buyButton.onClick.AddListener(() => TryPurchaseItem(item));
            buyButton.interactable = item.stock > 0;
        }
    }

    private void TryPurchaseItem(ShopItem item)
    {
        if (item.stock <= 0) return;

        int currentCoins = ScoreManager.Instance.GetScore();
        
        if (currentCoins >= item.price)
        {
            // Process purchase
            ScoreManager.Instance.RemoveCoins(item.price);
            item.stock--;
            playerInventory.AddItem(item.itemName);
            
            // Update UI
            UpdateCoinsDisplay();
            PopulateShop();
        }
        else
        {
            // Show "not enough coins" feedback
            StartCoroutine(ShowNotEnoughCoinsMessage());
            AudioManager.Instance?.PlaySound("Error");
        }
    }

    private IEnumerator ShowNotEnoughCoinsMessage()
    {
        if (notEnoughCoinsMessage != null)
        {
            notEnoughCoinsMessage.SetActive(true);
            yield return new WaitForSeconds(messageDisplayTime);
            notEnoughCoinsMessage.SetActive(false);
        }
    }

    private void UpdateCoinsDisplay()
    {
        if (playerCoinsText != null)
        {
            playerCoinsText.text = ScoreManager.Instance.GetScore().ToString();
        }
    }

    public void CloseShop()
    {
        if (isShopOpen)
        {
            isShopOpen = false;
            shopWindow.SetActive(false);
            menuManager.LockCursor();
        }
    }

// -----------------------------------------------------------------//

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            CloseShop();
        }
    }
}