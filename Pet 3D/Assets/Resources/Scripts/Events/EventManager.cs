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
    int maxEventsInQueue = 1;

    int eventCounter = 0;

    public Event availableEvent;

    public delegate void OnEventAvailable();
    public event OnEventAvailable onEventAvailable;

    private void Awake()
    {
        eventPrefab = Resources.Load<Event>("Prefabs/UI/Event");
        eventList = new List<Event>();

        if (Persistent.eventInfoList.Count > 0)
        {
            foreach (var eventInfo in Persistent.eventInfoList)
                AddEventToQueue(eventInfo);
        }
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
                UpdatePersistentEventList();
                break;
            }
        }
    }

    void UpdatePersistentEventList()
    {
        foreach (var e in eventList)
            Persistent.AddEvent(e.EventID, e.GetEventType(), e.EventStartTime);
    }

    void AddEventToQueue()
    {
        int eventsToAdd = maxEventsInQueue - eventList.Count;

        for (int i = 0; i < eventsToAdd; i++)
            eventList.Add(CreateNewRandomEvent());

        eventList.Sort(SortByEarliestTime);
    }

    void AddEventToQueue(EventInfo eventInfo)
    {
        eventList.Add(CreatePreSetEvent(eventInfo));
    }

    Event CreatePreSetEvent(EventInfo eventInfo)
    {
        return InstantiateAndInitializeEvent(eventInfo, false);
    }

    Event CreateNewRandomEvent()
    {
        return InstantiateAndInitializeEvent(null, true);
    }

    Event InstantiateAndInitializeEvent(EventInfo eventInfo, bool isRandom)
    {
        Event newEvent = Instantiate(eventPrefab, eventParentTransform);

        if (isRandom)
        {
            EventType eventType = (EventType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EventType)).Length - 1);
            newEvent.Initialize(this, eventType, DateTime.MinValue, eventCounter);
            eventCounter++;
        }
        else
        {
            newEvent.Initialize(this, eventInfo.eventType, eventInfo.startTime, eventInfo.ID);
        }

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

    void RemoveEventFromListAndDestroyGameObject(Event eventToRemove)
    {
        eventList.Remove(eventToRemove);
        Persistent.RemoveEvent(eventToRemove.EventID);
        Destroy(eventToRemove.gameObject);
    }

    public void StartEvent(Event eventToStart)
    {
        string eventName = eventToStart.EventName;

        RemoveEventFromListAndDestroyGameObject(eventToStart);

        if (eventName.Contains("Race"))
        {
            SceneManager.LoadScene("RaceScene");
        }
        else if (eventName.Contains("Skill"))
        {
            SceneManager.LoadScene("SkillContestScene");
        }
        else if (eventName.Contains("Battle"))
        {
            SceneManager.LoadScene("BattleScene");
        }
    }

    public void EventAvailable(Event eventAvailable)
    {
        availableEvent = eventAvailable;
        onEventAvailable();
    }

    public Event GetAvailableEvent()
    {
        return availableEvent;
    }
}
