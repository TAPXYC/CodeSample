namespace Tapxyc.GameData.PPSave
{
    using UnityEngine;

    class FloatPlayerPrefsHandler : BaseGenericPlayerPrefsHandler<float>
    {
        public FloatPlayerPrefsHandler(string playerPrefName, float defaultValue) : base(playerPrefName, defaultValue) { }


        protected override float GetValue()
        {
            return PlayerPrefs.GetFloat(PlayerPrefName, _defaultValue);
        }

        protected override void SetValue(float newValue)
        {
            PlayerPrefs.SetFloat(PlayerPrefName, newValue);
        }
    }
}