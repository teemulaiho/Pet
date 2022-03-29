using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EventManager : MonoBehaviour
{
    [SerializeField] Event eventPrefab;
    [SerializeField] Transform eventParentTransform;

    List<Event> eventList;
    int maxEventsInQueue = 6;

    private void Awake()
    {
        eventPrefab = Resources.Load<Event>("Prefabs/UI/Event");
        eventList = new List<Event>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (CheckEventQueue())
            UpdateEventQueue(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckEventQueue())
            UpdateEventQueue(0);
    }

    bool CheckEventQueue()
    {
        if (eventList.Count < maxEventsInQueue)
            UpdateEventQueue(0);

        return false;
    }

    /// <summary>
    /// Update the event queue.
    /// </summary>
    /// <param name="updateType">Update the event queue.</param>
    void UpdateEventQueue(int updateType)
    {
        switch (updateType)
        {
            case 0:
            {
                AddEventToQueue();
                break;
            }
        }
    }

    void AddEventToQueue()
    {
        int eventsToAdd = maxEventsInQueue - eventList.Count;

        for (int i = 0; i < eventsToAdd; i++)
            eventList.Add(CreateNewRandomEvent());

        eventList.Sort(SortByEarliestTime);
    }

    Event CreateNewRandomEvent()
    {
        Event newEvent = Instantiate(eventPrefab, eventParentTransform);
        EventType eventType = (EventType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EventType)).Length - 1);
        newEvent.Initialize(this, eventType);

        return newEvent;
    }

    public DateTime GetLastEventStartTime()
    {
        DateTime latestStartTime = DateTime.Now;

        foreach (var e in eventList)
            if (DateTime.Compare(latestStartTime, e.EventStartTime) < 0)
                latestStartTime = e.EventStartTime;

        return latestStartTime;
    }

    int SortByEarliestTime(Event e1, Event e2)
    {
        return DateTime.Compare(e1.EventStartTime, e2.EventStartTime);
    }

    public void StartEvent(Event eventToStart)
    {
        string eventName = eventToStart.EventName;

        if (eventName.Contains("Race"))
        {
            SceneManager.LoadScene("RaceScene");
        }
        else if (eventName.Contains("Skill Contest"))
        {
            SceneManager.LoadScene("SkillContestScene");
        }
        else if (eventName.Contains("Battle"))
        {
            SceneManager.LoadScene("BattleScene");
        }
    }
}
