using System;
using UnityEngine;
using Tapxyc.Monetizate.Reward;

public class GameReward : BaseReward
{
    [SerializeField] string AndroidAdUnitId = "YOUR_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account
    [SerializeField] string IOSAdUnitId = "YOUR_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account



    /// <summary>
    /// Каллбэк для вызова после показа рекламы
    /// </summary>
    private Action<bool> _currentCallback;
    private string _currentID;


    public void Init()
    {
#if UNITY_ANDROID
        _currentID = AndroidAdUnitId;
#endif

#if UNITY_IOS
            _currentID = IOSAdUnitId;
#endif
        InitializeRewardedAds(_currentID);
    }


    public void ShowReward(Action<bool> callback)
    {
        _currentCallback = callback;

        //Можно ли вообще показать рекламу
        bool result = ShowReward();

        //Если нельзя, вызываем с фолсом
        if (!result)
            _currentCallback?.Invoke(false);
    }


    //Если показалась реклама, да еще и до конца, то вызывает каллбэк с тру
    protected override void OnRewardedAdReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("REWARD RECIEVE");
        _currentCallback?.Invoke(true);
    }




    protected override void OnClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("CLICKED");
    }

    protected override void OnDisplay(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("DISPLAY");
    }

    protected override void OnFailedToDisplay(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("FAILED DISPLAY");
    }

    protected override void OnHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("HIDDEN");
    }

    protected override void OnLoadComplete(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("LOAD COMPLETE");
    }

    protected override void OnLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, int tryingNum)
    {
        LogMessage("LOAD FAIL");
    }

    protected override void OnPaidAdRevenue(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("PAID REVENUE");
    }
}