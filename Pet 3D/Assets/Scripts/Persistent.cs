using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public static class Persistent
{
    public static List<EventInfo> eventInfoList;
    public static InventorySystem inventorySystem;
    public static List<Item> itemDatabase;
    public static PlayerInventory playerInventory;
    public static PetStats petStats;

#if UNITY_EDITOR
    static Persistent()
    {
        Initialize();
    }
#endif

#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
    public static void Initialize()
    {
        eventInfoList = new List<EventInfo>();

        inventorySystem = new InventorySystem();
        itemDatabase = new List<Item>();

        playerInventory = new PlayerInventory
        {
            money = 100
        };

        petStats = new PetStats
        {
            name = "Player Pet",
            health = 100f,
            energy = 100f,
            stamina = 2,
            intellect = 2f,
            strength = 1f,
            experience = 0f
        };
    }

    public static void AddExperience(float experienceToAdd)
    {
        petStats.experience += experienceToAdd;
    }

    public static void AddEvent(int id, EventType type, DateTime eventStartTime)
    {
        foreach (var ei in eventInfoList)
            if (ei.ID == id)
                return;

        EventInfo newEvent = new EventInfo();
        newEvent.ID = id;
        newEvent.eventType = type;
        newEvent.startTime = eventStartTime;
        eventInfoList.Add(newEvent);
    }

    public static void RemoveEvent(int id)
    {
        EventInfo eventToRemove = null;
        foreach (var ei in eventInfoList)
            if (ei.ID == id)
                eventToRemove = ei;

        if (eventToRemove != null)
            eventInfoList.Remove(eventToRemove);
    }
}