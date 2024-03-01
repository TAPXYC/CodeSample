namespace Tapxyc.GameData.PPSave
{
    using System;
    using UnityEngine;

    /// <summary>
    /// ВНИМАНИЕ!!! НЕ ПОДХОДИТ ДЛЯ ПРОСТЫХ ТИПОВ ДАННЫХ, ВРОДЕ ИНТА ИЛИ СТРИНГА!! ТОЛЬКО ДЛЯ КЛАССОВ ИЛИ СТРУКТУР
    /// </summary>
    /// <typeparam name="T">Класс или структура</typeparam>
    public class PPGenericStorage<T> : BaseGenericPlayerPrefsHandler<T> where T : IComparable
    {
        public PPGenericStorage(string playerPrefName, T defaultValue) : base(playerPrefName, defaultValue) { }


        protected override T GetValue()
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(PlayerPrefName, JsonUtility.ToJson(_defaultValue)));
        }

        protected override void SetValue(T newValue)
        {
            PlayerPrefs.SetString(PlayerPrefName, JsonUtility.ToJson(newValue));
        }
    }
}