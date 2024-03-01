namespace Tapxyc.GameData.PPSave
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR
    using Tapxyc.Drawler;
#endif


    /// <summary>
    /// Only for int, float or string!
    /// </summary>
    public abstract class BaseGenericPlayerPrefsHandler<T> : BasePlayerPrefsHandler where T : IComparable
    {
        public event Action OnClear;

        /// <summary>
        /// Текущее значение
        /// </summary>
        public T Value
        {
            get
            {
                return _lastValue;
            }
            set
            {
                if (value.CompareTo(_lastValue) != 0)
                {
                    _lastValue = value;

                    if (AutoSave)
                        SetValue(_lastValue);

                    OnCheckUpdateValue?.Invoke(PlayerPrefName, _lastValue);
                    DebugX.ColorMessage($"{PlayerPrefName} save value {_lastValue}!", Color.yellow);
                }
            }
        }


        /// <summary>
        /// Первая ли инициализация значения
        /// </summary>
        public bool IsFirstInit
        {
            get;
            private set;
        }

        public bool AutoSave
        {
            get;
            set;
        } = true;


        protected T _defaultValue;


        private static event Action<string, object> OnCheckUpdateValue;

        private T _lastValue;
        private Func<T, T> DrawFunc;


        public BaseGenericPlayerPrefsHandler(string playerPrefName, T defaultValue) : base(playerPrefName)
        {
            InitTypeCode<T>();

#if UNITY_EDITOR
            SetDrawFunc(TypeCode);
#endif

            _defaultValue = defaultValue;

            IsFirstInit = _dontLoadOldData || !PlayerPrefs.HasKey(PlayerPrefName);

            _lastValue = _dontLoadOldData ? _defaultValue : GetValue();
            SetValue(_lastValue);

            OnCheckUpdateValue += CheckUpdateValue;
        }



        ~BaseGenericPlayerPrefsHandler()
        {
            OnCheckUpdateValue -= CheckUpdateValue;
        }




        #region Private

        private void CheckUpdateValue(string prefsName, object value)
        {
            if (prefsName == PlayerPrefName)
                _lastValue = (T)value;
        }

        #endregion






        #region Command

        public override void BaseReloadData()
        {
            Value = GetValue();
        }

        protected override void OnSave()
        {
            SetValue(_lastValue);
        }


        public override void Clear()
        {
            Value = _defaultValue;
            PlayerPrefs.DeleteKey(PlayerPrefName);
            OnClear?.Invoke();
        }



#if UNITY_EDITOR

        public override void Draw()
        {
            Value = DrawFunc(Value);
        }


        public override void SetDefault()
        {
            Value = _defaultValue;
        }


        private void SetDrawFunc(int typeCode)
        {
            if (typeCode == 0)
                DrawFunc = currentValue => (T)(object)StaticDrawler.IntField($"{PlayerPrefName}", (int)(object)currentValue);


            if (typeCode == 1)
                DrawFunc = currentValue => (T)(object)StaticDrawler.FloatField($"{PlayerPrefName}", (float)(object)currentValue);


            if (typeCode == 2)
                DrawFunc = currentValue => (T)(object)StaticDrawler.TextField($"{PlayerPrefName}", (string)(object)currentValue);

        }

#endif

        #endregion;





        #region Abstract

        protected abstract T GetValue();
        protected abstract void SetValue(T newValue);

        #endregion;

    }
}