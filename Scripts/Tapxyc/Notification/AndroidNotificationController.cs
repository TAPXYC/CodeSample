using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
using Unity.Notifications.Android;
#endif

public class AndroidNotificationController
{
    const int NotificationID = 10000;
    public AndroidNotificationController()
    {

    }

#if UNITY_ANDROID
    public void RequestPermission()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }


    public void RegistrationChanel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_1",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }


    public void SendNotification(string tittle, string text, DateTime fireTime)
    {
        var notification = new AndroidNotification();
        notification.Title = tittle;
        notification.Text = text;
        notification.FireTime = fireTime;
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "channel_1", NotificationID);
    }

    public void ClearNotification()
    {
        AndroidNotificationCenter.CancelNotification(NotificationID);
    }
#endif
}
