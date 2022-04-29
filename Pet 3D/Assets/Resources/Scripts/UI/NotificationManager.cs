using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    static Notification notificationPrefab;
    static Transform longNotificationParent;
    static Transform notificationParent;
    static Animator longNotificationAnimator;
    static Animator notificationAnimator;
    static List<Notification> longNotificationList;
    static List<Notification> notificationList;

    private void Awake()
    {
        notificationPrefab = Resources.Load<Notification>("Prefabs/UI/Notification");

        notificationParent = transform.GetChild(0);
        longNotificationParent = transform.GetChild(1);

        longNotificationAnimator = longNotificationParent.GetComponent<Animator>();
        notificationAnimator = notificationParent.GetComponent<Animator>();

        longNotificationParent.GetComponent<CanvasGroup>().alpha = 1f;

        longNotificationList = new List<Notification>();
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
            else if ((int)value == 4)
            {
                notification.AddText("\n New Skill: Pet can now be called with a Whistle!");
            }
            else if ((int)value == 6)
            {
                notification.AddText("\n New Skill: You can now charge up throwing speed!");
            }
        }
        else if (notificationType == NotificationType.NPCSpawn)
        {
            notification.Initialize("Mole has arrived! Maybe check out what he has to say?");
        }

        notificationAnimator.SetTrigger("Show");
        AddNotificationToLongNotificationList(notification);
    }

    static void AddNotificationToLongNotificationList(Notification notificationToAdd)
    {
        Notification longNotification = Instantiate(notificationPrefab, longNotificationParent);
        longNotification.SetText(notificationToAdd.GetText());
        longNotification.transform.SetParent(longNotificationParent);

        if (longNotificationList.Count >= 1)
        {
            foreach (var not in longNotificationList)
                Destroy(not.gameObject);

            longNotificationList.Clear();
        }

        longNotificationList.Add(longNotification);
        longNotificationAnimator.SetTrigger("Drop");
    }
}
