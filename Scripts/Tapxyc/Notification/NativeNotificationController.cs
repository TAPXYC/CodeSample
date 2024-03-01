using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeNotificationController : MonoBehaviour
{
    private AndroidNotificationController androidNotification;
    private IOSNotificationController iOSNotification;


    // Start is called before the first frame update
    public void Init()
    {
#if UNITY_ANDROID
        androidNotification = new AndroidNotificationController();

        androidNotification.RequestPermission();
        androidNotification.RegistrationChanel();
#elif UNITY_IOS
        iOSNotification = new IOSNotificationController();

        StartCoroutine(iOSNotification.RequestAuthorization());
#endif
    }

    public void SendNotification(string tittle, string text, DateTime fireTime)
    {
#if UNITY_ANDROID
        androidNotification.SendNotification(tittle, text, fireTime);
#elif UNITY_IOS
        iOSNotification.SendNotification(tittle, "", text, fireTime);
#endif
    }


    public void ClearNotification()
    {
#if UNITY_ANDROID
        androidNotification.ClearNotification();
#elif UNITY_IOS
        iOSNotification.ClearNotification();
#endif
    }
}
