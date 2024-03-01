/*
using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;
using Tapxyc.GameData.PPSave;
using System;
using System.Linq;

namespace Tapxyc.Analytics
{
    public class AppsFlyerEventsSender : MonoBehaviour
    {
        /// <summary>
        /// Таймер игрового времени
        /// </summary>
        [SerializeField] AnalyticsTimerService timerService;

        public static AppsFlyerEventsSender InstObj;

        public int CurrentGameplayTime => timerService.CompletedMinuts;
        public IEnumerator WaitForLoaded => new WaitUntil(() => _isLoaded);


        /// <summary>
        /// Номер события
        /// </summary>
        private IntPlayerPrefsHandler _progressEventNumber;

        /// <summary>
        /// Первая ли это покупка
        /// </summary>
        private BoolPlayerPrefsHandler _isFirstPurchase;
        private bool _isLoaded = false;


        private void Awake()
        {
            InstObj = this;

            timerService.Init();
            timerService.Begin();
            timerService.OnMinutePassed += MinutePassed;

            _progressEventNumber = new IntPlayerPrefsHandler("Analityc.Event_N", 1);
            _isFirstPurchase = new BoolPlayerPrefsHandler("Analityc.IsFirstPurchase", true);

            _isLoaded = true;
        }



#region Events

        #region Progress

        public void SendProgress(string buildingName, int buildingLevel)
        {
            Dictionary<string, string> progressEvent = new Dictionary<string, string>();
            progressEvent.Add("event_n", _progressEventNumber.Value++.ToString());
            progressEvent.Add("name", $"{buildingName}_{buildingLevel}");
            progressEvent.Add("time", timerService.CompletedMinuts.ToString());

            SendEvent("progress", progressEvent);
        }


        public void SendTutorialProgress(string tutorialName, int tutorialStep)
        {
            Dictionary<string, string> tutorialEvent = new Dictionary<string, string>();
            tutorialEvent.Add("step", tutorialStep.ToString());
            tutorialEvent.Add("step_name", tutorialName);
            tutorialEvent.Add("time", timerService.CompletedMinuts.ToString());
            tutorialEvent.Add("status", "completed");

            SendEvent("tutorial", tutorialEvent);
        }



        /// <summary>
        /// Отправка ивента на прошествие минуты геймплея
        /// </summary>
        /// <param name="currentMinute">Текущая минута геймплея</param>
        /// <param name="sessionNum">Номер сессии</param>
        private void MinutePassed(int currentMinute, int sessionNum)
        {
            Dictionary<string, string> timerEvent = new Dictionary<string, string>();
            timerEvent.Add("time", currentMinute.ToString());
            SendEvent("timer", timerEvent);
        }

        #endregion





        #region Purchase

        public void SendPurchase(decimal revenue, string currency, string productId, string storeProductId, string transactionId)
        {
            if (_isFirstPurchase.Value)
            {
                _isFirstPurchase.Value = false;
                SendFirstPurchase();
            }

            Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();
            purchaseEvent.Add("currency", currency);
            purchaseEvent.Add("revenue", revenue.ToString("0.00"));
            purchaseEvent.Add("sku", productId);
            purchaseEvent.Add("productId", storeProductId);
            purchaseEvent.Add("time", timerService.CompletedMinuts.ToString());
            purchaseEvent.Add("TRANSACTION_ID", transactionId);

            SendEvent("af_purchase", purchaseEvent);
        }


        private void SendFirstPurchase()
        {
            Dictionary<string, string> uniquepuEvent = new Dictionary<string, string>();
            uniquepuEvent.Add("time", timerService.CompletedMinuts.ToString());
            SendEvent("unique_pu", uniquepuEvent);
        }

        #endregion






        #region Ads

        /// <summary>
        /// Завершение просмотра интерстишела
        /// </summary>
        public void SendWatchInterstitialsComplete()
        {
            Dictionary<string, string> rvfinishEvent = new Dictionary<string, string>();
            rvfinishEvent.Add("time", timerService.CompletedMinuts.ToString());
            SendEvent("it_finish", rvfinishEvent);
        }



        /// <summary>
        /// Завершение просмотра реварда
        /// </summary>
        /// <param name="rewardName">Название реварда</param>
        public void SendCompleteWatchingRewardVideo(string rewardName)
        {
            Dictionary<string, string> rvfinishEvent = new Dictionary<string, string>();
            rvfinishEvent.Add("placement", rewardName);
            rvfinishEvent.Add("time", timerService.CompletedMinuts.ToString());
            SendEvent("rv_finish", rvfinishEvent);
        }

        #endregion

#endregion



        /// <summary>
        /// Отправка ивента
        /// </summary>
        /// <param name="eventName">Название ивента</param>
        /// <param name="parameters">Параметры</param>
        private void SendEvent(string eventName, Dictionary<string, string> parameters)
        {
            string stringParams = String.Join(", ", parameters.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            Debug.LogError($"[ANALITYC_EVENT]: {eventName} {{{stringParams}}}");

            AppsFlyer.sendEvent(eventName, parameters);
        }
    }
}
*/