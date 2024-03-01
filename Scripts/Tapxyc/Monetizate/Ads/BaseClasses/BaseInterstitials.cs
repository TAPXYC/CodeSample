namespace Tapxyc.Monetizate.Insterstitials
{
    using System;
    using UnityEngine;
    using static MaxSdkBase;

    public abstract class BaseInterstitials : MonoBehaviour
    {
        private string _adUnitId = "YOUR_AD_UNIT_ID";
        private int _retryAttempt;


        /// <summary>
        /// Инициализация событий и загрузка рекламы
        /// </summary>
        /// <param name="adUnitId">Идентификатор межстраничных объявлений. Берется из кабинета аппловина</param>
        protected void InitializeInterstitialAds(string adUnitId)
        {
            _adUnitId = adUnitId;

            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

            // Load the first interstitial
            LoadInterstitial();
        }




        /// <summary>
        /// Показать объвление
        /// </summary>
        /// <returns>Удалось ли показать объявление</returns>
        protected bool ShowInterstitial()
        {
            bool canShow = MaxSdk.IsInterstitialReady(_adUnitId);

            if (canShow)
            {
                MaxSdk.ShowInterstitial(_adUnitId);
                LogMessage("Показ рекламы");
            }
            else
            {
                LogMessage("Не удалось показать объявление");
            }

            return canShow;
        }




        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(_adUnitId);
        }



        #region Events handlers

        /// <summary>
        /// Объявление загрузилось
        /// </summary>
        private void OnInterstitialLoadedEvent(string adUnitId, AdInfo adInfo)
        {
            // Межстраничное объявление готово для показа. MaxSdk.IsInterstitialReady(adUnitId) теперь возвращает «true»

            // Сброс счетчика повторных попыток
            _retryAttempt = 0;
            OnLoadComplete(adUnitId, adInfo);
        }




        /// <summary>
        /// Объявление не смогло загрузиться
        /// </summary>
        private void OnInterstitialLoadFailedEvent(string adUnitId, ErrorInfo errorInfo)
        {
            // AppLovin рекомендует повторить попытку с экспоненциально более высокими задержками, вплоть до максимальной задержки (в данном случае 64 секунды).

            _retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));

            Invoke("LoadInterstitial", (float)retryDelay);

            OnLoadFailed(adUnitId, errorInfo, _retryAttempt);
        }





        /// <summary>
        /// Объявление показалось
        /// </summary>
        private void OnInterstitialDisplayedEvent(string adUnitId, AdInfo adInfo)
        {
            OnDisplay(adUnitId, adInfo);
        }




        /// <summary>
        /// Объявление не удалось показать
        /// </summary>
        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, ErrorInfo errorInfo, AdInfo adInfo)
        {
            // Не удалось отобразить межстраничное объявление. AppLovin рекомендует загрузить следующую рекламу.
            LoadInterstitial();

            OnFailedToDisplay(adUnitId, errorInfo, adInfo);
        }




        /// <summary>
        /// Объявление показалось
        /// </summary>
        private void OnInterstitialClickedEvent(string adUnitId, AdInfo adInfo)
        {
            OnClicked(adUnitId, adInfo);
        }



        /// <summary>
        /// Объявление закрылось
        /// </summary>
        private void OnInterstitialHiddenEvent(string adUnitId, AdInfo adInfo)
        {
            LoadInterstitial();
            OnHidden(adUnitId, adInfo);
        }

        #endregion





        protected void LogMessage(string message)
        {
            DebugX.ColorMessage($"{DebugX.ColorPart("[INTERSTITIALS]", Color.cyan)}: {DebugX.ColorPart(message, Color.yellow)}");
        }




        #region Abstract

        /// <summary>
        /// Объявление загрузилось
        /// </summary>
        protected abstract void OnLoadComplete(string adUnitId, AdInfo adInfo);



        /// <summary>
        /// Объявление не смогло загрузиться
        /// </summary>
        protected abstract void OnLoadFailed(string adUnitId, ErrorInfo errorInfo, int tryingNum);


        /// <summary>
        /// Объявление показалось
        /// </summary>
        protected abstract void OnDisplay(string adUnitId, AdInfo adInfo);


        /// <summary>
        /// Объявление не удалось показать
        /// </summary>
        protected abstract void OnFailedToDisplay(string adUnitId, ErrorInfo errorInfo, AdInfo adInfo);


        /// <summary>
        /// Объявление закрылось
        /// </summary>
        protected abstract void OnHidden(string adUnitId, AdInfo adInfo);


        protected abstract void OnClicked(string adUnitId, AdInfo adInfo);

        #endregion
    }
}