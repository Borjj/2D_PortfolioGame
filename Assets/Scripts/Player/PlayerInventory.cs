using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject[] potionSprites = new GameObject[3]; // Array of potion UI images
    private int potionCount = 0;


// ---------------------------------------------------------------------------------- //
    public void AddItem(string itemName)
    {
        if (itemName == "Potion" && potionCount < 3) // Max 3 potions
        {
            potionSprites[potionCount].SetActive(true);
            potionCount++;
        }
    }


// ------------------------------------------------------------------------------------ //
// POTION METHODS //
    public int GetPotionCount()
    {
        return potionCount;
    }

    public void UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            potionSprites[potionCount].SetActive(false);
        }
    }
}