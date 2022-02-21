using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Dictionary<string, int> playerInventory;
    public static PlayerInventory current;

    private void Awake()
    {
        playerInventory = new Dictionary<string, int>();
        current = this;
    }

    public event Action onInventoryValueChange;
    public void InventoryValueChange()
    {
        if (onInventoryValueChange != null)
        {
            onInventoryValueChange();
        }
    }
    public bool HasItemInventory(string itemToCheck)
    {
        return playerInventory.ContainsKey(itemToCheck) && playerInventory[itemToCheck] > 0;
    }

    public void AddItemToPlayerInventory(string newItem)
    {
        if (playerInventory.ContainsKey(newItem))
        {
            playerInventory[newItem] += 1;
        }
        else
        {
            playerInventory.Add(newItem, 1);
        }

        InventoryValueChange();
    }

    public void RemoveItemFromPlayerInventory(string itemToBeRemoved)
    {
        if (playerInventory.ContainsKey(itemToBeRemoved) &&
            playerInventory[itemToBeRemoved] > 0)
        {
            playerInventory[itemToBeRemoved] -= 1;
        }

        InventoryValueChange();
    }

    public int GetInventoryCount(string itemToCheck)
    {
        if (playerInventory.ContainsKey(itemToCheck))
            return playerInventory[itemToCheck];
        else
            return 0;
    }
}
