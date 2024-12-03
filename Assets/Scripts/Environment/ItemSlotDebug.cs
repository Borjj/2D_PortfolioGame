using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotDebug : MonoBehaviour
{
    void Start()
    {
        // Check all required components
        Transform iconObj = transform.Find("Icon");
        Transform nameObj = transform.Find("ItemName");
        Transform quantityObj = transform.Find("Quantity");

        Debug.Log("=== ItemSlot Component Check ===");
        
        // Check Icon
        if (iconObj != null)
        {
            Image iconImage = iconObj.GetComponent<Image>();
            Debug.Log($"Icon Object: Found, Has Image Component: {iconImage != null}");
        }
        else Debug.Log("Icon Object: Missing");

        // Check ItemName
        if (nameObj != null)
        {
            TextMeshProUGUI nameText = nameObj.GetComponent<TextMeshProUGUI>();
            Debug.Log($"ItemName Object: Found, Has TMP Component: {nameText != null}");
        }
        else Debug.Log("ItemName Object: Missing");

        // Check Quantity
        if (quantityObj != null)
        {
            TextMeshProUGUI quantityText = quantityObj.GetComponent<TextMeshProUGUI>();
            Debug.Log($"Quantity Object: Found, Has TMP Component: {quantityText != null}");
        }
        else Debug.Log("Quantity Object: Missing");
    }
}