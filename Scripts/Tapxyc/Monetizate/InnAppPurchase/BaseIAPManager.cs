
/*namespace Tapxyc.Monetizate.IAP
{
    using UnityEngine;
    using UnityEngine.Purchasing;
    using Tapxyc.GameData.PPSave;
    using System.Linq;
    using System.Collections.Generic;


    /// <summary>
    /// Объект покупки
    /// </summary>
    public struct PurchaseItem
    {
        /// <summary>
        /// ID элемента покупки. ВНИМАТЕЛЬНО ПРОВЕРЯЙТЕ НА СОВПАДЕНИЕ
        /// </summary>
        public readonly string ProductID;

        /// <summary>
        /// Тип покупки. Подписки и постоянные (отключение рекламы) покупки восстанавливаются (не восстанавливаются покупки предметов)
        /// </summary>
        public readonly ProductType ProductType;


        public PurchaseItem(string productID, ProductType productType)
        {
            ProductID = productID;
            ProductType = productType;
        }
    }





    public abstract class BaseIAPManager : MonoBehaviour, IStoreListener
    {   
        [Header("Покупка без перехода в магазин")]
        [SerializeField] bool test;


        /// <summary>
        /// Список внутриигровых покупок
        /// </summary>
        private IEnumerable<PurchaseItem> _purchaseItems;

        /// <summary>
        /// Магазин
        /// </summary>
        private IStoreController m_StoreController;

        /// <summary>
        /// При первом входе в приложение пытается восстановить покупки
        /// </summary>
        private BoolPlayerPrefsHandler _isFirstRunHandler;



        /// <summary>
        /// !!!ОБЯЗАТЕЛЬНО Инициализация менеджера покупок из потомков
        /// </summary>
        /// <param name="purchaseItems">Все предметы для покупки</param>
        protected void InitIAP(IEnumerable<PurchaseItem> purchaseItems)
        {
            _purchaseItems = purchaseItems;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var item in _purchaseItems)
                builder.AddProduct(item.ProductID, item.ProductType);

            UnityPurchasing.Initialize(this, builder);

            Message($"Init proccess complete");
        }



        /// <summary>
        /// Начать покупку продукта
        /// </summary>
        /// <param name="productID">ID желаемого продукта</param>
        public void BuyProduct(string productID)
        {
#if UNITY_EDITOR
            if (test)
            {
                Message($"Test purchase Complete - Product: {productID}");
                OnBuyProduct(null, productID);
            }
            else
#endif

            if (m_StoreController != null)
                m_StoreController.InitiatePurchase(productID);
        }




        #region Interface override

        /// <summary>
        /// Успешная инициализация сервера покупок. см. метод InitializePurchaseServer(IEnumerable<PurchaseItem> purchaseItems)
        /// Проверка на восстановление покупок
        /// </summary>
        /// <param name="controller">Контроллер покупок</param>
        /// <param name="extensions"></param>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Message("In-App Purchasing successfully initialized");
            m_StoreController = controller;

            CheckRestoreProducts();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Message($"In-App Purchasing initialize failed: {error}");
        }




        /// <summary>
        /// Совершение покупки
        /// </summary>
        /// <param name="args">информация о купленом продукте</param>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Message($"Purchase Complete - Product: {args.purchasedProduct.definition.id}");
            OnBuyProduct(args, args.purchasedProduct.definition.id);

            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Message($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        #endregion





        #region Private


        /// <summary>
        /// Проверка, нужно ли восстановление покупок
        /// </summary>
        private void CheckRestoreProducts()
        {
            _isFirstRunHandler = new BoolPlayerPrefsHandler("IAP.IsFirstStart", true);

            if (_isFirstRunHandler.Value)
            {
                _isFirstRunHandler.Value = false;
                RestoreMyProduct();

                Message($"Restore proccess complete");
            }
            else
                Message($"Restore proccess not require");
        }



        /// <summary>
        /// Восстановление покупок
        /// </summary>
        private void RestoreMyProduct()
        {
            var notAConsumableProducts = _purchaseItems.Where(pi => pi.ProductType != ProductType.Consumable);

            foreach (var product in notAConsumableProducts)
            {
                try
                {
                    if (m_StoreController.products.WithID(product.ProductID).hasReceipt)
                        OnRestoreProduct(product.ProductID);
                }
                catch
                {
                    Message($"Cant restore product [{product.ProductID}]");
                }
            }
        }


        protected void Message(string message)
        {
            DebugX.ColorMessage(DebugX.ColorPart("[IAP_MANAGER]", Color.green) + ": " + message);
        }

        #endregion




        #region Abstract

        /// <summary>
        /// Успешная покупка
        /// </summary>
        /// <param name="productInfo">Информация о продукте</param>
        /// <param name="productID">ID продукта</param>
        protected abstract void OnBuyProduct(PurchaseEventArgs productInfo, string productID);

        /// <summary>
        /// Успешное восстановление покупки
        /// </summary>
        /// <param name="productID">ID продукта</param>
        protected abstract void OnRestoreProduct(string productID);

        #endregion
    }
}*/