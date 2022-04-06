using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    static Notification notificationPrefab;
    static Transform notificationParent;
    static Animator notificationAnimator;
    static List<Notification> notificationList;

    private void Awake()
    {
        notificationPrefab = Resources.Load<Notification>("Prefabs/UI/Notification");
        notificationParent = transform.GetChild(0);
        notificationAnimator = notificationParent.GetComponent<Animator>();
        notificationList = new List<Notification>();
    }

    public static void ReceiveNotification(NotificationType notificationType, float value)
    {
        Notification notification;

        if (notificationList.Count == 0)
        {
            notification = Instantiate(notificationPrefab, notificationParent);
            notificationList.Add(notification);
        }
        else
            notification = notificationList[0];

        if (notificationType == NotificationType.Experience)
            notification.Initialize("Nice Catch! " + "+" + value.ToString() + "XP");
        else if (notificationType == NotificationType.LevelUp)
        {
            notification.Initialize("Intellect leveled up to " + value.ToString() + "!");

            if ((int)value == 2)
            {
                notification.AddText("\n New Skill: Pet can now catch the ball!");
            }
        }

        notificationAnimator.SetTrigger("Show");
    }
}
