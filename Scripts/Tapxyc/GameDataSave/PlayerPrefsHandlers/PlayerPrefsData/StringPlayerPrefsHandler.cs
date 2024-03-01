namespace Tapxyc.GameData.PPSave
{
    using UnityEngine;

    class StringPlayerPrefsHandler : BaseGenericPlayerPrefsHandler<string>
    {
        public StringPlayerPrefsHandler(string playerPrefName, string defaultValue) : base(playerPrefName, defaultValue) { }


        protected override string GetValue()
        {
            return PlayerPrefs.GetString(PlayerPrefName, _defaultValue);
        }

        protected override void SetValue(string newValue)
        {
            PlayerPrefs.SetString(PlayerPrefName, newValue);
        }
    }
}