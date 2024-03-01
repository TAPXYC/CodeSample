namespace Tapxyc.Monetizate.Reward
{
    
    using System;
    using UnityEngine;
    using static MaxSdkBase;

    public abstract class BaseReward : MonoBehaviour
    {
        public bool RewardAwailable => MaxSdk.IsRewardedAdReady(_adUnitId);


        private string _adUnitId = "YOUR_AD_UNIT_ID";
        private int _retryAttempt;


        /// <summary>
        /// ОБЯЗАТЕЛЬНО! Вызывать от потомка один раз
        /// </summary>
        /// <param name="adUnitId">Идентификатор рекламы ревардов. Берется в кабинете аппловина</param>
        protected void InitializeRewardedAds(string adUnitId)
        {
            _adUnitId = adUnitId;

            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first rewarded ad
            LoadRewardedAd();
        }


        /// <summary>
        /// Начать показ рекламы
        /// </summary>
        /// <returns>Сможем ли мы показать рекламу</returns>
        protected bool ShowReward()
        {
            bool canShow = RewardAwailable;

            if (canShow)
            {
                MaxSdk.ShowRewardedAd(_adUnitId);
                LogMessage("Показ реварда");
            }
            else
            {
                LogMessage("Не удалось показать ревард");
            }

            return canShow;
        }






        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(_adUnitId);
        }




        private void OnRewardedAdLoadedEvent(string adUnitId, AdInfo adInfo)
        {
            // Ревард загружен для показа. MaxSdk.IsRewardedAdReady(adUnitId) теперь вернет 'true'.

            // Сброс счетчика попыток
            _retryAttempt = 0;
            OnLoadComplete(adUnitId, adInfo);
        }




        private void OnRewardedAdLoadFailedEvent(string adUnitId, ErrorInfo errorInfo)
        {
            // Не удалось загрузить ревард
            // AppLovin рекомендует повторить попытку с экспоненциально более высокими задержками, вплоть до максимальной задержки (в данном случае 64 секунды).

            _retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));

            Invoke("LoadRewardedAd", (float)retryDelay);
            OnLoadFailed(adUnitId, errorInfo, _retryAttempt);
        }




        private void OnRewardedAdDisplayedEvent(string adUnitId, AdInfo adInfo)
        {
            OnDisplay(adUnitId, adInfo);
        }



        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, ErrorInfo errorInfo, AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();
            OnFailedToDisplay(adUnitId, errorInfo, adInfo);
        }



        private void OnRewardedAdClickedEvent(string adUnitId, AdInfo adInfo)
        {
            OnClicked(adUnitId, adInfo);
        }



        private void OnRewardedAdHiddenEvent(string adUnitId, AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
            OnHidden(adUnitId, adInfo);
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, Reward reward, AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            OnRewardedAdReceived(adUnitId, reward, adInfo);
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
            OnPaidAdRevenue(adUnitId, adInfo);
        }





        protected void LogMessage(string message)
        {
            DebugX.ColorMessage($"{DebugX.ColorPart("[REWARD]", Color.cyan)}: {DebugX.ColorPart(message, Color.yellow)}");
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



        /// <summary>
        /// Выплачен доход от рекламы. Используйте этот обратный вызов для отслеживания доходов пользователей.
        /// </summary>
        protected abstract void OnPaidAdRevenue(string adUnitId, AdInfo adInfo);

        
        /// <summary>
        /// Ревард получен
        /// </summary>
        protected abstract void OnRewardedAdReceived(string adUnitId, Reward reward, AdInfo adInfo);

        #endregion
    }
}