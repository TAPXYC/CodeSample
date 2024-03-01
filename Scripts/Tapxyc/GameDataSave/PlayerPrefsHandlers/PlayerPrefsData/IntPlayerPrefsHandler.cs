namespace Tapxyc.GameData.PPSave
{
    using UnityEngine;



    public class IntPlayerPrefsHandler : BaseGenericPlayerPrefsHandler<int>
    {
        public IntPlayerPrefsHandler(string playerPrefName, int defaultValue) : base(playerPrefName, defaultValue) { }


        protected override int GetValue()
        {
            return PlayerPrefs.GetInt(PlayerPrefName, _defaultValue);
        }

        protected override void SetValue(int newValue)
        {
            PlayerPrefs.SetInt(PlayerPrefName, newValue);
        }
    }
}