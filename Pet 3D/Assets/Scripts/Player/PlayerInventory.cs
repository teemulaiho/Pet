using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static Dictionary<string, int> playerInventory;

    private void Awake()
    {
        playerInventory = new Dictionary<string, int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool HasItemInventory(string itemToCheck)
    {
        return playerInventory.ContainsKey(itemToCheck);
    }

    public static void AddItemToPlayerInventory(string newItem)
    {
        if (playerInventory.ContainsKey(newItem))
        {
            playerInventory[newItem] += 1;
        }
        else
        {
            playerInventory.Add(newItem, 1);
        }
    }

    public static void RemoveItemFromPlayerInventory(string itemToBeRemoved)
    {
        if (playerInventory.ContainsKey(itemToBeRemoved) &&
            playerInventory[itemToBeRemoved] > 0)
        {
            playerInventory[itemToBeRemoved] -= 1;
        }
    }
}
