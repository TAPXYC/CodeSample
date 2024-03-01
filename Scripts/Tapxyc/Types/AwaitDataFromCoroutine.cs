namespace Tapxyc.Types
{
    using System;
    using System.Collections;
    using UnityEngine;


    public class AwaitDataFromCoroutine<T>
    {
        public T Data
        {
            get;
            private set;
        }

        public event Action<T> OnComplete;

        public IEnumerator WaitForComplete => new WaitUntil(() => _isCompleted);

        private bool _isCompleted = false;

        public AwaitDataFromCoroutine(T defaultData = default(T))
        {
            Reset(defaultData);
        }

        public void Reset(T defaultData = default(T))
        {
            Data = defaultData;
            _isCompleted = false;
        }

        public void Complete(T data)
        {
            Data = data;
            OnComplete?.Invoke(Data);
            
            _isCompleted = true;
        }
    }

}

