namespace Tapxyc.Types
{
    using System;

    public class RealTimeHandler
    {
        public float CurrentSeconds => (float)(DateTime.UtcNow - _startTime).TotalSeconds;
        public float CurrentMilliseconds => (float)(DateTime.UtcNow - _startTime).TotalMilliseconds;

        private DateTime _startTime;

        public RealTimeHandler()
        {
            Reset();
        }


        public void Reset()
        {
            _startTime = DateTime.UtcNow;
        }
    }
}