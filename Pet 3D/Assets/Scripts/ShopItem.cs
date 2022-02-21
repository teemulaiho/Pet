using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] Item item;

    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text itemText;

    public void UpdateShopItem(Item newItem)
    {
        item = newItem;

        itemIcon.sprite = item.GetItemSprite();
        itemText.text = item.GetItemDescription();
    }
}
