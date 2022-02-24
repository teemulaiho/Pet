using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public ItemSlot[] itemSlots;

    private int selectionIndex;
    public InventoryItem selectedItem;

    public void Init()
    {
        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].Init();

        selectionIndex = -1;
    }

    private void Update()
    {
        int newIndex = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            newIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            newIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            newIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            newIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            newIndex = 4;

        if (newIndex >= 0)
        {
            if (newIndex == selectionIndex)
            {
                itemSlots[newIndex].Deselect();
                selectedItem = null;
                selectionIndex = -1;
            }
            else
            {
                if (selectionIndex >= 0)
                {
                    itemSlots[selectionIndex].Deselect();
                    selectedItem = null;
                }

                itemSlots[newIndex].Select();
                if (itemSlots[newIndex].inventoryItem != null)
                    selectedItem = itemSlots[newIndex].inventoryItem;

                selectionIndex = newIndex;
            }
        }
    }

    public void AssignItemToIndex(InventoryItem inventoryItem, int index)
    {
        itemSlots[index].AssignItem(inventoryItem);
    }
}
