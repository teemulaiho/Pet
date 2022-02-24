using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public static class Persistent
{
    public static List<Item> itemDatabase;
    public static PlayerInventory playerInventory;
    public static PetStats petStats;
   
    #if UNITY_EDITOR
    static Persistent()
    {
        Initialize();
    }
    #endif

    #if UNITY_STANDALONE
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    #endif
    public static void Initialize()
    {
        itemDatabase = new List<Item>();

        playerInventory = new PlayerInventory
        {
            money = 20
        };

        petStats = new PetStats
        {
            name = "Player Pet",
            health = 100f,
            energy = 100f,
            stamina = 2
        };
    }
}