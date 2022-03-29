using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class EventInfo
{
    public int ID { get; set; }
    public EventType eventType { get; set; }
    public DateTime startTime { get; set; }
}

public class Event : MonoBehaviour
{
    int eventID;
    EventType eventType;
    EventManager eventManager;

    DateTime currentTime;
    [SerializeField] DateTime startTime;

    [SerializeField] string eventName;
    [SerializeField] float timeLeft;
    [SerializeField] int hoursLeft;
    [SerializeField] int minutesLeft;
    [SerializeField] int secondsLeft;
    bool countingDown;

    [SerializeField] int startHour;
    [SerializeField] int startMinute;

    [Space]
    [Header("UI Elements")]
    [SerializeField] TMP_Text uiTextEventName;
    [SerializeField] TMP_Text uiTextTimeLeftUntilEventOpens;
    [SerializeField] Button uiButtonStartEvent;
    [SerializeField] Slider uiBarTimer;

    public int EventID { get { return eventID; } set { eventID = value; } }
    public string EventName { get { return eventName; } }
    public DateTime EventStartTime { get { return startTime; } set { startTime = value; } }
    public EventType GetEventType() { return eventType; }
    public void SetEventType(EventType type) { eventType = type; }

    public void Initialize(EventManager em, EventType type, DateTime eventStartTime, int id)
    {
        eventID = id;
        eventManager = em;
        eventType = type;
        eventName = eventType.ToString();
        SetEventStartTime(eventStartTime);
    }

    private void Awake()
    {
        currentTime = DateTime.Now;
        startTime = DateTime.Now;
        uiButtonStartEvent.onClick.AddListener(StartEvent);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetEventUIInfo();
        countingDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!countingDown)
            return;

        UpdateUI();
    }

    void UpdateUI()
    {
        UpdateEventUIText();
    }

    void UpdateEventUIText()
    {
        timeLeft = (float)startTime.Subtract(DateTime.Now).TotalSeconds;

        if (timeLeft <= 0)
        {
            uiButtonStartEvent.interactable = true;
            countingDown = false;
        }

        minutesLeft = (int)(timeLeft / 60);
        secondsLeft = (int)(timeLeft % 60);
        hoursLeft = (minutesLeft / 60);
        minutesLeft %= 60;

        if (hoursLeft < 10)
        {
            uiTextTimeLeftUntilEventOpens.text = "0";
            uiTextTimeLeftUntilEventOpens.text += hoursLeft.ToString();
        }
        else
        {
            uiTextTimeLeftUntilEventOpens.text = hoursLeft.ToString();
        }

        uiTextTimeLeftUntilEventOpens.text += "h ";

        if (minutesLeft < 10)
        {
            uiTextTimeLeftUntilEventOpens.text += "0";
            uiTextTimeLeftUntilEventOpens.text += minutesLeft.ToString();
        }
        else
        {
            uiTextTimeLeftUntilEventOpens.text += minutesLeft.ToString();
        }

        uiTextTimeLeftUntilEventOpens.text += "m ";

        if (secondsLeft < 10)
        {
            uiTextTimeLeftUntilEventOpens.text += "0";
            uiTextTimeLeftUntilEventOpens.text += secondsLeft.ToString();
        }
        else
        {
            uiTextTimeLeftUntilEventOpens.text += secondsLeft.ToString();
        }

        uiTextTimeLeftUntilEventOpens.text += "s";
    }

    void SetEventUIInfo()
    {
        uiTextEventName.text = eventType.ToString();
        uiButtonStartEvent.interactable = false;
    }

    void SetEventStartTime(DateTime eventStartTime)
    {
        if (eventStartTime != DateTime.MinValue) // using MinValue instead of null -check. -Teemu
            startTime = eventStartTime;
        else
            startTime = GetNewStartTime();
    }

    void StartEvent()
    {
        eventManager.StartEvent(this);
    }

    DateTime GetNewStartTime()
    {
        DateTime newStartTime = eventManager.GetLastEventStartTime();
        int curHour = currentTime.Hour;
        int curMin = currentTime.Minute;

        startHour = curHour + UnityEngine.Random.Range(0, 0);
        startMinute = curMin + UnityEngine.Random.Range(0, 60 - curMin);

        int hoursToAdd = startHour - curHour;
        int minutesToAdd = startMinute - curMin;

        newStartTime = newStartTime.AddHours(hoursToAdd);
        newStartTime = newStartTime.AddMinutes(minutesToAdd);

        return newStartTime;
    }
}
