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

    public static void ReceiveNotification(NotificationType notificationType)
    {
        if (notificationType == NotificationType.Experience)
        {
            if (notificationList.Count == 0)
            {
                Notification notification = Instantiate(notificationPrefab, notificationParent);
                notification.Initialize("Nice Catch! + 50xp");
                notificationList.Add(notification);
                
            }
            else
            {
                notificationList[0].Initialize("Nice Catch! + 50xp");
            }

            notificationAnimator.SetTrigger("Show");
        }
    }
}
