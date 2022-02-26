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
    List<Item> items;
}
