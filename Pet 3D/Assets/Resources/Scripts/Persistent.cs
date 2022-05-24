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
    public static ItemDatabase itemDatabase;
    public static PlayerInventory playerInventory;
    public static PetStats petStats;
    public static PetSkills petSkills;
    public static bool initialLoad = true;
    public static int loadCount = 0;

//#if UNITY_EDITOR
//    static Persistent()
//    {
//        Initialize();
//    }
//#endif

#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif

    public static void Initialize()
    {
        eventInfoList = new List<EventInfo>();

        itemDatabase = new ItemDatabase();
        playerInventory = new PlayerInventory
        {
            money = 100
        };

        petStats = new PetStats
        {
            name = "Pet",
            level = 1,
            health = 100f,
            hunger = 100f,
            energy = 100f,
            stamina = 2,
            intellect = 1f,
            strength = 1f
        };

        petSkills = new PetSkills
        {
            //throwSkill = false,
            //catchSkill = false,
            //whistleSkill = false
        };

        petSkills.Init();

        Application.targetFrameRate = 70;
        QualitySettings.vSyncCount = 0;

        if (initialLoad)
        {
            //// initialize database on game launch
            //if (itemDatabase.items.Count == 0)
            //{
            //    itemDatabase.items.Add(Resources.Load<Item>("ScriptableObjects/AppleItem"));
            //    itemDatabase.items.Add(Resources.Load<Item>("ScriptableObjects/BallItem"));
            //    itemDatabase.items.Add(Resources.Load<Item>("ScriptableObjects/WhistleItem"));
            //    itemDatabase.items.Add(Resources.Load<Item>("ScriptableObjects/TetherBallItem"));
            //}

            ////Persistent.playerInventory.AddItem(Persistent.itemDatabase.ItemByName("Apple"), 5);
            //playerInventory.AddItem(itemDatabase.ItemByName("Ball"), 5);
            ////Persistent.playerInventory.AddItem(Persistent.itemDatabase.ItemByName("Whistle"), 1);

            initialLoad = false;
        }

        loadCount++;
    }

    public static void AddIntellect(float intellectToAdd)
    {
        petStats.intellect += intellectToAdd;

        if (petSkills.CheckForSkillUnlock(SkillRequirement.Intellect, (int)petStats.intellect))
            NotificationManager.ReceiveNotification(NotificationType.LevelUp, petStats.intellect);
    }

    public static bool CheckIfSkillUnlocked(string skillToCheck)
    {
        return (petSkills._skillDictionary[skillToCheck]._unlocked);
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