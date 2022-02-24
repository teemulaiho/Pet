using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
