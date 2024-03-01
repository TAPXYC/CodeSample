using System;
using UnityEngine;

namespace Tapxyc.GameData.PPSave
{
    public enum TypeCode
    {
        NotRegister = -1,
        Int,
        Float,
        String
    }




    public abstract class BasePlayerPrefsHandler : BaseDataEditorCommand
    {
        /// <summary>
        /// Название в PlayerPrefs
        /// </summary>
        public readonly string PlayerPrefName;



        public int TypeCode
        {
            get;
            private set;
        }



        private static event Action OnClearAll;
        protected static bool _dontLoadOldData = false;



        protected BasePlayerPrefsHandler(string playerPrefName)
        {
            PlayerPrefName = playerPrefName;
            OnClearAll += Clear;
        }


        protected override void BaseOnSaveData()
        {
            OnSave();
        }




        protected void InitTypeCode<T>()
        {
            TypeCode = GetTypeCode<T>();
            CheckValidType(typeof(T).ToString());
        }




        private void CheckValidType(string type)
        {
            if (TypeCode == -1)
                DebugX.ColorMessage($"{type} is wrong type for PlayerPrefs! Если так задумано и нет конфликтов - все нормально, забей");
        }





        private int GetTypeCode<T>()
        {
            int typeCode = -1;

            if (typeof(T) == typeof(int))
                typeCode = 0;


            if (typeof(T) == typeof(float))
                typeCode = 1;


            if (typeof(T) == typeof(string))
                typeCode = 2;

            return typeCode;
        }



        public static void ClearAll()
        {
            OnClearAll?.Invoke();
        }

        public static void DontLoadOldData()
        {
            _dontLoadOldData = true;
            Debug.LogError("DONT LOAD OLD DATA");
        }


        protected abstract void OnSave();
        public abstract void Clear();


    }
}