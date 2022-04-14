using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hotbar : MonoBehaviour
{
    [SerializeField] TMP_Text playerMoney;

    public HotbarItemSlot[] itemSlots;

    private int selectionIndex;

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
        {
            itemSlots[i].Init();
            if (i < Persistent.playerInventory.hotbar.Count)
                AssignItemToSlot(Persistent.playerInventory.hotbar[i], i);
        }

        selectionIndex = -1;
    }

    private void Start()
    {
        Persistent.playerInventory.onMoneyChange += UpdatePlayerInfo;
        UpdatePlayerInfo();
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
                    selectionIndex = -1;
                }
                else
                {
                    if (selectionIndex >= 0)
                    {
                        itemSlots[selectionIndex].Deselect();
                    }

                    selectionIndex = i;

                    itemSlots[selectionIndex].Select();

                }
                break;
            }
        }

        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].UpdateSlot();
    }

    public InventoryItem GetSelectedItem()
    {
        if (selectionIndex >= 0)
            return itemSlots[selectionIndex].inventoryItem;

        return null;
    }

    public void AssignItemToSlot(InventoryItem inventoryItem, int index)
    {
        itemSlots[index].AssignItem(inventoryItem);
    }

    public void UpdateSlots()
    {

    }

    public void UpdatePlayerInfo()
    {
        if (playerMoney)
            playerMoney.text = Persistent.playerInventory.money.ToString();
    }
}
