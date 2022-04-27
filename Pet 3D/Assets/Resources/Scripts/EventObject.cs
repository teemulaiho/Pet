using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    UIController uiController;
    EventWindow eventWindow;
    bool _isOpen;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
        eventWindow = FindObjectOfType<EventWindow>();
    }

    private void Start()
    {
        if (!eventWindow && uiController)
            eventWindow = uiController.GetEventWindow();

            if (eventWindow)
            eventWindow.onWindowClose += OnWindowClose;
    }

    public void Toggle()
    {
        if (uiController)
        {
            if (!_isOpen)
                _isOpen = uiController.OpenUIWindow(this.gameObject);
            else
                _isOpen = uiController.CloseUIWindw(this.gameObject);
        }
    }

    void OnWindowClose(bool isOpen)
    {
        _isOpen = isOpen;
    }
}
