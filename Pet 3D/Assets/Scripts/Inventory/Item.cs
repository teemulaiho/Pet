using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Default,
    Spawnable,
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public GameObject prefab;
    [TextArea(5, 10)]
    [SerializeField] string description;

    public string GetItemDescription() { return description; }
}
