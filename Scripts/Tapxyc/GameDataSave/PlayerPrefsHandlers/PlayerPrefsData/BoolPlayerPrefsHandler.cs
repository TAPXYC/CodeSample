namespace Tapxyc.GameData.PPSave
{
    using UnityEngine;

    public class BoolPlayerPrefsHandler : BaseGenericPlayerPrefsHandler<bool>
    {
        public BoolPlayerPrefsHandler(string playerPrefName, bool defaultValue) : base(playerPrefName, defaultValue) { }



        protected override bool GetValue()
        {
            return PlayerPrefs.GetInt(PlayerPrefName, _defaultValue ? 1 : 0) == 1;
        }


        protected override void SetValue(bool newValue)
        {
            PlayerPrefs.SetInt(PlayerPrefName, newValue ? 1 : 0);
        }
    }
}