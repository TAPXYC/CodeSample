using System.Collections;
using UnityEngine;
using Tapxyc.Monetizate.Insterstitials;
using Tapxyc.Analytics;



/// <summary>
/// Рассчитано так, что действует вместе с аналитикой. Если аналитика не нужна, просто удалить _analytics отовсюду. На функционал не повлияет
/// </summary>
public class GameInterstitials : BaseInterstitials
{
    [SerializeField] string adUnitId = "YOUR_AD_UNIT_ID";
    [Space]
    [SerializeField] int noInterstitialsTime = 10;
    [SerializeField] int interstitialsInterval = 120;



    /// <summary>
    /// Готово ли к показу
    /// </summary>
    private bool _readyToShow = false;

    /// <summary>
    /// Отправщик аналитики
    /// </summary>
    //private AppsFlyerEventsSender _analytics;


    
    public void Init(/*AppsFlyerEventsSender analytics*/)
    {
        //_analytics = analytics;
        InitializeInterstitialAds(adUnitId);

        StartCoroutine(CheckTimeForInterstitialsCorr());
    }


    public void TryShowInterstitials()
    {
        if (_readyToShow)
        {
            _readyToShow = false;
            ShowInterstitial();
        }
    }




    private IEnumerator CheckTimeForInterstitialsCorr()
    {
        //Ждем пока аналитика загрузится
        //yield return _analytics.WaitForLoaded;

        //Ждем пока начальное время игры меньше, чем указано (например первые 10 минут геймплея не показывать рекламу)
        //yield return new WaitWhile(() => _analytics.CurrentGameplayTime < noInterstitialsTime);

        while (true)
        {
            LogMessage("Interstitials is Ready");
            _readyToShow = true;
            yield return new WaitUntil(() => !_readyToShow);
            yield return new WaitForSecondsRealtime(interstitialsInterval + 30);
        }
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
        LogMessage("FAILED TO DISPLAY");
    }

    protected override void OnHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("HIDDEN");
        //_analytics.SendWatchInterstitialsComplete();
    }

    protected override void OnLoadComplete(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LogMessage("LOAD COMPLETE");
    }

    protected override void OnLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, int tryingNum)
    {
        LogMessage("LOAD FAILED");
    }
}