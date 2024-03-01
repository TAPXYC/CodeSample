using System;
//using Facebook.Unity;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    /*
    void Awake()
    {
        try
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }

        }
        catch (Exception e)
        {
            ScreenMessageController.ScreenLog("FACEBOOK init error: " + e.Message, Color.red);
        }


        DontDestroyOnLoad(gameObject);
    }




    // Unity will call OnApplicationPause(false) when an app is resumed
    // from the background
    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }
        }
    }
    */
}
