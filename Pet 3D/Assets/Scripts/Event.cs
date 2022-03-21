using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    EventManager eventManager;

    [SerializeField] DateTime startTime;
    
    [SerializeField] string eventName;
    [SerializeField] float timeLeft;
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

    public string EventName { get { return eventName; } }

    public void Initialize(EventManager em)
    {
        eventManager = em;
    }

    private void Awake()
    {
        startTime = DateTime.Now;
        uiButtonStartEvent.onClick.AddListener(StartEvent);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetEventUIInfo();
        SetEventStartTime();

        countingDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!countingDown)
            return;

        timeLeft = (float)startTime.Subtract(DateTime.Now).TotalSeconds;

        if (timeLeft <= 0)
        {
            uiButtonStartEvent.interactable = true;
            countingDown = false;
        }

        minutesLeft = (int)(timeLeft / 60);
        secondsLeft = (int)(timeLeft % 60);

        if (minutesLeft < 10)
        {
            uiTextTimeLeftUntilEventOpens.text = "0";
            uiTextTimeLeftUntilEventOpens.text += minutesLeft.ToString();
        }
        else
        {
            uiTextTimeLeftUntilEventOpens.text = minutesLeft.ToString();
        }

        uiTextTimeLeftUntilEventOpens.text += ":";

        if (secondsLeft < 10)
        {
            uiTextTimeLeftUntilEventOpens.text += "0";
            uiTextTimeLeftUntilEventOpens.text += secondsLeft.ToString();
        }
        else
        {
            uiTextTimeLeftUntilEventOpens.text += secondsLeft.ToString();
        }
    }

    void SetEventUIInfo()
    {
        uiTextEventName.text = eventName;
        uiButtonStartEvent.interactable = false;
    }

    void SetEventStartTime()
    {
        int curHour = startTime.Hour;
        int curMin = startTime.Minute;

        int hoursToAdd = startHour - curHour;
        int minutesToAdd = startMinute - curMin;

        startTime = startTime.AddHours(hoursToAdd);
        startTime = startTime.AddMinutes(minutesToAdd);
    }

    void StartEvent()
    {
        eventManager.StartEvent(this);
    }
}
