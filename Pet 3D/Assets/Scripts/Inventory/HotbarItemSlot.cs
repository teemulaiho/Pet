using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarItemSlot : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] Image image;
    [SerializeField] TMP_Text count;

    public InventoryItem inventoryItem;

    public void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        UnassignItem();
        Deselect();
    }

    public void UpdateSlot()
    {
        if (inventoryItem != null)
        {
            if (inventoryItem.count == 0)
                UnassignItem();
            else
            {
                count.text = "x" + inventoryItem.count.ToString();
            }
        }
    }

    public void AssignItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
        image.sprite = inventoryItem.item.icon;
        image.enabled = true;
        count.text = "x" + inventoryItem.count.ToString();
    }
    public void UnassignItem()
    {
        inventoryItem = null;
        image.enabled = false;
        count.text = null;
        inventoryItem = null;
    }

    public void Select()
    {
        canvasGroup.alpha = 1.0f;
    }
    public void Deselect()
    {
        canvasGroup.alpha = 0.5f;
    }
}
