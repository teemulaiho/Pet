using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Inventory/Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Default,
        Spawnable,
        Usable
    }

    public string itemName;
    public Sprite icon;
    public ItemType type;
    public GameObject prefab;
    public int cost;
    [TextArea(5, 10)]
    [SerializeField] string description;

    public string GetItemDescription() { return description; }
}