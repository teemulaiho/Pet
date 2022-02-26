using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public List<InventoryItem> inventory;
    public int money;

    public PlayerInventory()
    {
        inventory = new List<InventoryItem>();
        money = 0;
    }

    public bool Contains(Item item)
    {
        foreach (InventoryItem inventoryItem in inventory)
            if (inventoryItem.item == item)
                return true;

        return false;
    }

    public void AddItem(Item item, int amount)
    {
        if (Contains(item))
        {
            foreach (InventoryItem inventoryItem in inventory)
                if (inventoryItem.item == item)
                {
                    inventoryItem.count += amount;
                    break;
                }
        }
        else
        {
            inventory.Add(new InventoryItem(item, amount));
        }
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
            if (inventory[i].item == item)
            {
                inventory[i].count -= 1;
                if (inventory[i].count == 0)
                {
                    inventory[i] = null;
                    inventory.Remove(inventory[i]);
                }

                break;
            }
    }

    public int GetItemCount(Item item)
    {
        if (Contains(item))
        {
            foreach (InventoryItem inventoryItem in inventory)
                if (inventoryItem.item == item)
                    return inventoryItem.count;
        }

        return 0;
    }
}
