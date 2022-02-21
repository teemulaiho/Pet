using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] string itemName;
    [TextArea(5, 10)]
    [SerializeField] string itemDesctiption;
    [SerializeField] SpriteRenderer itemSpriteRenderer;
    [SerializeField] Sprite itemSprite;


    public string GetItemName()
    {
        return itemName;
    }

    public string GetItemDescription()
    {
        return itemDesctiption;
    }

    public Sprite GetItemSprite()
    {
        if (itemSpriteRenderer)
            return itemSpriteRenderer.sprite;
        else
            return itemSprite;
    }
}
