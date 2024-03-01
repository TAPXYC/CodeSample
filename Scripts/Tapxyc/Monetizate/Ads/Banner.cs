namespace Tapxyc.Monetizate
{
    using Tapxyc.Types;
    using UnityEngine;

    public class Banner : MonoBehaviour
    {
        [SerializeField] string AndroidAdUnitId = "YOUR_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account
        [SerializeField] string IOSAdUnitId = "YOUR_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account

        private bool _bannerEnabled = false;
        private float _screenWidth;
        private string _currentID;

        //Рассчетчик зависимости высоты баннера от ШИРИНЫ экрана. Подобрано экспериментально. 
        //Если надо изменить - x - ширина экрана, y - высота баннера. 
        //Выводить функцию сложно и вряд ли она универсальна, поэтому использована линейная интерполяция по точкам
        private LinePathCalculator _bannerHeightCalculator = new LinePathCalculator(new Vector3[]
                {
                    new Vector3(0, 600),
                    new Vector3(400, 443),
                    new Vector3(720, 292),
                    new Vector3(1080, 191),
                    new Vector3(1242, 156),
                    new Vector3(1440, 140),
                    new Vector3(5000, 130),
                });




        /// <summary>
        /// ОБЯЗАТЕЛЬНО! Инициализация баннера. Вызывать один раз
        /// </summary>
        public void InitializeBannerAds()
        {
#if UNITY_ANDROID
            _currentID = AndroidAdUnitId;
#endif

#if UNITY_IOS
            _currentID = IOSAdUnitId;
#endif

            MaxSdk.CreateBanner(_currentID, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerExtraParameter(_currentID, "adaptive_banner", "true");

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(_currentID, new Color(0.9450981f, 0.9450981f, 0.9450981f, 1));
            MaxSdk.StartBannerAutoRefresh(_currentID);

            _screenWidth = Camera.main.pixelWidth;
            SetBannerEnable(true);
        }

        public void SetBannerEnable(bool enable)
        {
            if (_currentID.IsNullOrEmpty())
                return;

            if (enable)
            {
                if (!_bannerEnabled)
                    MaxSdk.ShowBanner(_currentID);
            }
            else
            {
                if (_bannerEnabled)
                    MaxSdk.HideBanner(_currentID);
            }

            _bannerEnabled = enable;
        }





        public float GetHeight()
        {
            /*float bannerHeight = 0;

            if (_bannerEnabled)
            {
                //Передаем в рассчетчик ширину экрана, находим значение по заданной ширине и берем значение высоты экрана (y)
                bannerHeight = _bannerHeightCalculator.GetPointAtDistanceX(_screenWidth, out _).y;
            }

            return bannerHeight;*/
            return _bannerEnabled ? MaxSdkUtils.GetAdaptiveBannerHeight() : 0;
        }
    }
}