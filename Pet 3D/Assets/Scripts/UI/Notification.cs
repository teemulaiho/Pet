using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum NotificationType
{
    Experience,
    Count
}

public class Notification : MonoBehaviour
{
    [SerializeField] TMP_Text notificationText;


    public void Initialize(string text)
    {
        notificationText.text = text;
    }

    void Awake()
    {
        notificationText = GetComponentInChildren<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
