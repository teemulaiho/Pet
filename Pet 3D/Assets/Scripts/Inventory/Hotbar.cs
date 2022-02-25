using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public ItemSlot[] itemSlots;

    private int selectionIndex;
    public InventoryItem selectedItem;

    private KeyCode[] keyCodes;

    public void Init()
    {
        keyCodes = new KeyCode[5];
        keyCodes[0] = KeyCode.Alpha1;
        keyCodes[1] = KeyCode.Alpha2;
        keyCodes[2] = KeyCode.Alpha3;
        keyCodes[3] = KeyCode.Alpha4;
        keyCodes[4] = KeyCode.Alpha5;

        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].Init();

        selectionIndex = -1;
    }

    private void Update()
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                if (i == selectionIndex)
                {
                    itemSlots[selectionIndex].Deselect();
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

                    selectionIndex = i;

                    itemSlots[selectionIndex].Select();
                    if (itemSlots[selectionIndex].inventoryItem != null)
                        selectedItem = itemSlots[selectionIndex].inventoryItem;
                    
                }
                break;
            }
        }
    }
    public void UpdateSlot()
    {
        if (selectionIndex >= 0)
        {
            itemSlots[selectionIndex].UpdateSlot();
            if (itemSlots[selectionIndex].inventoryItem == null)
                selectedItem = null;
        }
    }

    public void AssignItemToSlot(InventoryItem inventoryItem, int index)
    {
        itemSlots[index].AssignItem(inventoryItem);
    }
}
