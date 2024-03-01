using System;
using Tapxyc.Analytics;
using Tapxyc.GameData.PPSave;
using Tapxyc.Monetizate;
using UnityEngine;


/// <summary>
/// Главный класс, контролирующий взаимодействие с аппловином
/// Сейчас устроено так, что для интерстишелов нужна аналитика (собственный класс). Если она не нужна, то удалить её из Init и из производного класса интерстишела
/// 
/// ВНИМАНИЕ!!! Не DontDestroyObject! Если надо, можно прописать.
/// ВНИМАНИЕ!!! НУЖНО вызывать метод Init извне, в точке входа в приложение.Если нет централизации в проекте, можете сделать синглтоном 
/// </summary>
public class MaxSDKController : MonoBehaviour
{
    [SerializeField] GameInterstitials interstitials;
    [SerializeField] GameReward rewards;
    [SerializeField] Banner banner;
    [Header("Тот же ключ, что и в окне аппловина")]
    [SerializeField] string SDK_key;


    public float BannerHeight => banner == null ? 0 : banner.GetHeight();
    public bool AdsIsEnable => _adsIsEnable.Value;
    public bool ForceAdsIsEnable => _forceAdsIsEnable.Value;

    /// <summary>
    /// Доступна ли вся реклама (false - значит отключена)
    /// </summary>
    private BoolPlayerPrefsHandler _adsIsEnable;

    /// <summary>
    /// Доступна ли принудительная реклама (false - значит отключены ТОЛЬКО интерстишелы)
    /// </summary>
    private BoolPlayerPrefsHandler _forceAdsIsEnable;


    public void Init(/*AppsFlyerEventsSender analytics*/)
    {
        _adsIsEnable = new BoolPlayerPrefsHandler("EnableAds", true);
        _forceAdsIsEnable = new BoolPlayerPrefsHandler("EnableForceAds", true);

        if (_adsIsEnable.Value)
        {
            //Подписка на завершение инициалихации рекламы
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                // AppLovin SDK is initialized, start loading ads
                if(interstitials != null)
                    interstitials.Init(/*analytics*/);

                if(rewards != null)
                    rewards.Init();

                if(banner != null)
                    banner.InitializeBannerAds();
            };

            MaxSdk.SetSdkKey(SDK_key);

            //Сюда помещать рекламные ID тестовых устройств (На андроиде "Настройки => Google => Реклама" и в самом низу строка в виде этого 374cf171-c0b4-4e93-ad98-d398-d3b55d799e15)
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[]
            {
            });

            //Начало инициализации
            MaxSdk.InitializeSdk();
        }
    }


    /// <summary>
    /// Показ интерстишела
    /// </summary>
    public void TryShowInterstitials()
    {
        if (_adsIsEnable.Value && _forceAdsIsEnable.Value)
            interstitials.TryShowInterstitials();
    }



    /// <summary>
    /// Показ реварда. В калбэк передавать метод обработки успеха просмотра реварда 
    /// </summary>
    public void TryShowReward(Action<bool> callback)
    {
        if (_adsIsEnable.Value)
            rewards.ShowReward(callback);
        else
            callback?.Invoke(true);
    }


    /// <summary>
    /// Отключение ВСЕЙ рекламы
    /// </summary>
    public void StopAds()
    {
        _adsIsEnable.Value = false;
        banner.SetBannerEnable(false);
    }



    /// <summary>
    /// Отключение интерстишелов
    /// </summary>
    public void StopForceAds()
    {
        _forceAdsIsEnable.Value = false;
    }




    /// <summary>
    /// Отрисовка кнопки показа дебаг консоли аппловина
    /// </summary>
    void OnGUI()
    {
        if (GameManager.ShowDebug)
        {
            if (GUI.Button(new Rect(Camera.main.pixelWidth - 170, 10, 160, 40), "ShowDebugConsole"))
                MaxSdk.ShowMediationDebugger();
        }
    }
}