/*
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using Tapxyc.Analytics;
using System.Linq;
using Tapxyc.Monetizate.IAP;



public class IAPManager : BaseIAPManager
{
    public event Action OnBuyNoAds;
    public event Action OnBuyNoForceAds;
    public event Action<int> OnBuyResourceBox;


    private AppsFlyerEventsSender _analytics;
    private MaxSDKController _maxSDKManager;


    //Для удобства лучше все покупки выписать в константы
    private const string noForceAds = "noforceads";
    private const string noAds = "noads";
    private const string air1 = "airdrop1";
    private const string air5 = "airdrop5";
    private const string air12 = "airdrop12";
    private const string air30 = "airdrop30";
    private const string air75 = "airdrop75";
    private const string air200 = "airdrop200";


    /// <summary>
    /// !!!ОБЯЗАТЕЛЬНО Проинициализировать извне. Сразу передаются аналитика и контроллер рекламы для отправки аналитики и отключения рекламы соответственно
    /// </summary>
    /// <param name="analytics">Контроллер аналитики. Можно убрать если не нужна</param>
    /// <param name="maxSDKManager">Контроллек рекламы. Также можно убрать</param>
    public void Init(AppsFlyerEventsSender analytics, MaxSDKController maxSDKManager)
    {
        _analytics = analytics;
        _maxSDKManager = maxSDKManager;

        PurchaseItem[] purchaseItems =
        {
                //покупаются один раз и восстанавливаются
                new PurchaseItem(noForceAds, ProductType.NonConsumable),
                new PurchaseItem(noAds, ProductType.NonConsumable),

                //покупаются сколько угодно раз. не восстанавливаются
                new PurchaseItem(air1, ProductType.Consumable),
                new PurchaseItem(air5, ProductType.Consumable),
                new PurchaseItem(air12, ProductType.Consumable),
                new PurchaseItem(air30, ProductType.Consumable),
                new PurchaseItem(air75, ProductType.Consumable),
                new PurchaseItem(air200, ProductType.Consumable),
            };

        InitIAP(purchaseItems);
    }



    /// <summary>
    /// Обработка покупки
    /// </summary>
    /// <param name="productInfo">ИНформация о продукте</param>
    /// <param name="productID">ID купленного продукта</param>
    protected sealed override void OnBuyProduct(PurchaseEventArgs productInfo, string productID)
    {
        CheckPurchase(productID);
        var product = productInfo.purchasedProduct;

        _analytics.SendPurchase(product.metadata.localizedPrice, product.metadata.isoCurrencyCode,
                                product.metadata.localizedTitle, productID, product.transactionID);
    }


    /// <summary>
    /// Обработка восстановления
    /// </summary>
    /// <param name="productID">ID восстановленной покупки</param>
    protected sealed override void OnRestoreProduct(string productID)
    {
        if (productID == noAds)
            Product_NoAds();

        if (productID == noForceAds)
            Product_NoForceAds();
    }



    #region Обработки покупок

    private void CheckPurchase(string purchaseID)
    {
        if (purchaseID == noForceAds)
            Product_NoForceAds();
        else if (purchaseID == noAds)
            Product_NoAds();
        else
        {
            int boxCount;
            int.TryParse(string.Join("", purchaseID.Where(c => char.IsDigit(c))), out boxCount);

            Debug.LogError(boxCount);
            Product_BuyResourceBox(boxCount);
        }
    }



    private void Product_NoAds()
    {
        OnBuyNoAds?.Invoke();
        _maxSDKManager.StopAds();
    }

    private void Product_NoForceAds()
    {
        OnBuyNoForceAds?.Invoke();
        _maxSDKManager.StopForceAds();
    }

    private void Product_BuyResourceBox(int count)
    {
        OnBuyResourceBox?.Invoke(count);
    }

    #endregion
}*/