namespace Tapxyc.Analytics
{
    using System;
    using System.Collections;
    using Tapxyc.GameData.PPSave;
    using UnityEngine;


    /// <summary>
    /// Данные времени аналитики
    /// </summary>
    [Serializable]
    public class AnalitycData : IComparable
    {
        public long FirstRunDate = 0;
        public double TotalTimeStorage = 0;
        public int LastReportedMinutes = 0;
        public int SessionsNum = 0;
        public long LastSessionTime = 0;


        public int CompareTo(object obj)
        {
            var other = obj as AnalitycData;

            bool totTime = TotalTimeStorage == other.TotalTimeStorage;
            bool lastMinTime = LastReportedMinutes == other.LastReportedMinutes;

            if (totTime && lastMinTime)
                return 0;
            else
                return 1;
        }
    }





    /// <summary>
    /// Service for calculation of game time since start of the game, counting sessions and dispatching events for analytics
    /// </summary>
    public class AnalyticsTimerService : MonoBehaviour
    {
        public int CompletedMinuts => _analitycHandler.Value.LastReportedMinutes;
        public int CurrentSessionNum => _analitycHandler.Value.SessionsNum;

        private const int SecondsPerMinute = 60;
        private const int MinSessionPauseMin = 5;
        private const int MaxTrackingMinutes = 600;


        // minutes of the first hour, session number
        public delegate void MinutePassedHandler(int currentMinute, int sessionNum);

        public event MinutePassedHandler OnMinutePassed;

        private PPGenericStorage<AnalitycData> _analitycHandler;

        private double _totalTime;
        private float _prevTime;

        private long _lastSessionTime;

        private bool _isSessionActive;
        private bool _isInitialized;


        public void Init()
        {
            DontDestroyOnLoad(gameObject);

            _analitycHandler = new PPGenericStorage<AnalitycData>("AnalitycTimer", new AnalitycData());
            _analitycHandler.OnClear += () => _isSessionActive = false;
        }



        public void Begin()
        {
            Message("Begin");

            // if no time stored, reset to now
            if (_analitycHandler.Value.FirstRunDate == 0)
            {
                _analitycHandler.Value.FirstRunDate = DateTime.UtcNow.Ticks;
                _analitycHandler.Save();
            }
            else
                CheckRightData();

            _isInitialized = true;

            // report session started
            SessionStart();

            // report minutes changed
            ReportMinutes();

            StartCoroutine(CheckSeconds());
        }





        private void ReportMinutes()
        {
            var minutes = (int)(_totalTime / SecondsPerMinute);

            if (_analitycHandler.Value.LastReportedMinutes != minutes && minutes < MaxTrackingMinutes)
            {
                _analitycHandler.Value.LastReportedMinutes = minutes;
                _analitycHandler.Save();

                OnMinutePassed?.Invoke(minutes, _analitycHandler.Value.SessionsNum);

                Message(DebugX.ColorPart("ReportMinutes", Color.yellow) + $" OnMinutePassed minutes = {minutes} sessionsNum = {_analitycHandler.Value.SessionsNum}");
            }
        }





        private void CheckRightData()
        {

        }



        private void Message(string message)
        {
            DebugX.ColorMessage(DebugX.ColorPart("[AnalyticsTimerService]", Color.yellow) + ": " + message);
        }


        private IEnumerator CheckSeconds()
        {
            while (true)
            {
                yield return new WaitWhile(() => !_isSessionActive || !_isInitialized);

                var current = Time.realtimeSinceStartup;
                var dt = current - _prevTime;
                _prevTime = current;

                _totalTime += dt;

                ReportMinutes();

                yield return null;
            }
        }



        private void OnApplicationPause(bool pauseStatus)
        {
            if (!_isInitialized) return;

            if (!pauseStatus)
            {
                SessionStart();
            }
            else
            {
                SessionEnd();
            }
        }




        private void SessionStart()
        {
            if (_isSessionActive)
                return;

            Message("Start");

            _prevTime = Time.realtimeSinceStartup;

            _totalTime = _analitycHandler.Value.TotalTimeStorage;
            _lastSessionTime = _analitycHandler.Value.LastSessionTime;

            var now = DateTime.UtcNow;
            CheckRightData();

            if ((int)(now - new DateTime(_lastSessionTime, DateTimeKind.Utc)).TotalMinutes >= MinSessionPauseMin)
            {
                _analitycHandler.Value.SessionsNum += 1;
                _analitycHandler.Save();
                ReportMinutes();
            }

            _isSessionActive = true;
        }




        private void SessionEnd()
        {
            if (!_isSessionActive)
                return;

            Message("End");

            _analitycHandler.Value.TotalTimeStorage = _totalTime;

            _analitycHandler.Value.LastSessionTime = DateTime.UtcNow.Ticks;
            _analitycHandler.Save();
            CheckRightData();

            _isSessionActive = false;
        }



        private void OnDestroy()
        {
            Message("Shutdown");

            // reset of session time
            SessionEnd();

            GameObject.Destroy(gameObject);
        }
    }
}