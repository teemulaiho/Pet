using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum NotificationType
{
    Experience,
    LevelUp,
    Count
}

public class Notification : MonoBehaviour
{
    [SerializeField] TMP_Text notificationText;

    void Awake()
    {
        notificationText = GetComponentInChildren<TMP_Text>();
    }


    public void Initialize(string text)
    {
        notificationText.text = text;
    }

    public void SetText(string text)
    {
        notificationText.text = text;
    }

    public void AddText(string textToAdd)
    {
        notificationText.text += textToAdd;
    }
}
