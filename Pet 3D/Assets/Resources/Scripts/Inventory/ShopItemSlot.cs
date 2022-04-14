using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    public delegate void SelectDelegate(ShopItemSlot shopItemSlot);
    private SelectDelegate select;

    public Item item;

    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text itemCost;
    [SerializeField] Image tint;
    
    private Color normalColor;
    private Color highlighted;

    private void Awake()
    {
        normalColor = tint.color;
        highlighted = tint.color;
        highlighted.a = 0.0f;
    }

    public void AssignItem(Item newItem)
    {
        item = newItem;

        itemIcon.sprite = item.icon;
        itemCost.text = item.cost.ToString() + "$";
        itemIcon.enabled = true;
        itemCost.enabled = true;
    }

    public void UnassignItem()
    {
        item = null;
        itemIcon.enabled = false;
        itemCost.enabled = false;
    }

    public void Select()
    {
        select?.Invoke(this);
    }

    public void Highlight(bool value)
    {
        if (value)
        {
            tint.color = highlighted;
        }
        else
        {
            tint.color = normalColor;
        }
    }

    public void AssignDelegate(SelectDelegate selectDelegate)
    {
        select = selectDelegate;
    }
}
