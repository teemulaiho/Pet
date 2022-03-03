using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public List<InventoryItem> inventory;
    public List<InventoryItem> hotbar;
    private int hotbarSize;
    public int money;

    public PlayerInventory()
    {
        inventory = new List<InventoryItem>();
        hotbar = new List<InventoryItem>();
        hotbarSize = 5;
        money = 0;
    }

    public bool Contains(Item item)
    {
        if (HotbarContains(item) || InventoryContains(item))
            return true;

        return false;
    }

    private bool HotbarContains(Item item)
    {
        foreach (InventoryItem inventoryItem in hotbar)
            if (inventoryItem.item == item)
                return true;

        return false;
    }
    private bool InventoryContains(Item item)
    {
        foreach (InventoryItem inventoryItem in inventory)
            if (inventoryItem.item == item)
                return true;

        return false;
    }

    public void AddItem(Item item, int amount)
    {
        if (!AddToHotbar(item, amount))
            AddToInventory(item, amount);
    }

    private bool AddToHotbar(Item item, int amount)
    {
        foreach (InventoryItem inventoryItem in hotbar)
            if (inventoryItem.item == item)
            {
                inventoryItem.count += amount;
                return true;
            }
        if (hotbar.Count < hotbarSize)
        {
            hotbar.Add(new InventoryItem(item, amount));
            return true;
        }
        return false;
    }

    private bool AddToInventory(Item item, int amount)
    {
        foreach (InventoryItem inventoryItem in inventory)
            if (inventoryItem.item == item)
            {
                inventoryItem.count += amount;
                return true;
            }

        hotbar.Add(new InventoryItem(item, amount));
        return true;
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < hotbar.Count; i++)
            if (hotbar[i].item == item)
            {
                hotbar[i].count -= 1;
                if (hotbar[i].count == 0)
                {
                    hotbar[i] = null;
                    hotbar.Remove(hotbar[i]);
                }
                break;
            }
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
        foreach (InventoryItem inventoryItem in hotbar)
            if (inventoryItem.item == item)
                return inventoryItem.count;

        foreach (InventoryItem inventoryItem in inventory)
            if (inventoryItem.item == item)
                return inventoryItem.count;

        return 0;
    }
}
