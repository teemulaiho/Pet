using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem
{

}

public class InventoryItem
{
    public Item item;
    public int count;

    public InventoryItem(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

public class ItemDatabase
{
    public List<Item> items;

    public ItemDatabase()
    {
        items = new List<Item>();
    }

    public Item ItemByName(string name)
    {
        foreach (Item item in items)
            if (item.itemName == name)
                return item;

        return null;
    }
}
