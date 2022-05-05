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

    public delegate void OnMoneyChange();
    public event OnMoneyChange onMoneyChange;

    public delegate void OnHotbarChange();
    public event OnHotbarChange onHotbarChange;

    private int Money
    {
        get { return money; }
        set
        {
            money = value;

            if (onMoneyChange != null)
                onMoneyChange();

            if (money <= 0)
                NotificationManager.ReceiveNotification(NotificationType.Money, money);
        }
    }

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

    public void AddItem(Item item, int amount = 1)
    {
        if (!AddToHotbar(item, amount))
            AddToInventory(item, amount);
    }

    private bool AddToHotbar(Item item, int amount)
    {
        bool hotbarChange = false;

        foreach (InventoryItem inventoryItem in hotbar)
            if (inventoryItem.item == item)
            {
                inventoryItem.count += amount;
                //return true;
                hotbarChange = true;
            }
        if (hotbar.Count < hotbarSize)
        {
            hotbar.Add(new InventoryItem(item, amount));
            //return true;
            hotbarChange = true;
        }

        if (hotbarChange && onHotbarChange != null)
            onHotbarChange();

        return hotbarChange;
        //return false;
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

        if (GetItemCount(item) <= 0)
            NotificationManager.ReceiveNotification(NotificationType.Inventory, 0);
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

    public void DecreaseMoney(int amountToDecrease)
    {
        Money -= amountToDecrease;
    }

    public void IncreaseMoney(int amountToIncrease)
    {
        Money += amountToIncrease;
    }
}
