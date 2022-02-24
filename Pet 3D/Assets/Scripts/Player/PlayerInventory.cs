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

    public void AddItemToPlayerInventory(Item item)
    {
        if (Contains(item))
        {
            foreach (InventoryItem inventoryItem in inventory)
                if (inventoryItem.item == item)
                {
                    inventoryItem.count += 1;
                    break;
                }
        }
        else
        {
            inventory.Add(new InventoryItem(item, 1));
        }
    }

    public void RemoveItemFromPlayerInventory(Item item)
    {
        if (Contains(item))
        {
            foreach (InventoryItem inventoryItem in inventory)
                if (inventoryItem.item == item)
                {
                    inventoryItem.count -= 1;
                    if (inventoryItem.count == 0)
                        inventory.Remove(inventoryItem);

                    break;
                }
        }
    }

    public int GetInventoryCount(Item item)
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
