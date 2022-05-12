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

    static int inventoryNotificationCount;
    static int moneyNotificationCount;

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
        bool showNotification = true;
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

            PetSkill skillUnlocked = Persistent.petSkills.GetLatestSkillUnlocked();

            if (skillUnlocked != null)
                notification.AddText($"\n New Skill: {skillUnlocked._description}");
        }
        else if (notificationType == NotificationType.NPCSpawn)
        {
            notification.Initialize("Mole has arrived! Maybe check out what he has to say?");
        }
        else if (notificationType == NotificationType.Inventory)
        {
            if (value <= 0 && inventoryNotificationCount < 2)
            {
                notification.Initialize("Item ran out? Not to worry, you can buy more from the shop.");
                inventoryNotificationCount++;
            }
            else showNotification = false;
        }
        else if (notificationType == NotificationType.Money)
        {
            if (value <= 0 && moneyNotificationCount < 2)
            {
                notification.Initialize("Low on money? Try to win some money from the events.");
                moneyNotificationCount++;
            }
            else
                showNotification = false;
        }

        if (showNotification)
        {
            notificationAnimator.SetTrigger("Show");
            AddNotificationToLongNotificationList(notification);
        }
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
