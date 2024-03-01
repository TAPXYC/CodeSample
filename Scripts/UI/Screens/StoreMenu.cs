using System;
using UnityEngine;
using UnityEngine.Purchasing;
using Gravitons.UI.Modal;
using UnityEngine.Purchasing.Extension;
using System.Linq;

[Serializable]
public struct PurchaseItem
{
    public string ID;
    public int DiamondCount;
    public int CardCount;
}


public class StoreMenu : MonoBehaviour
{
    [SerializeField] GameObject cards;
    [SerializeField] GameObject diamonds;
    [Space]
    [SerializeField] GameObject baseCards;
    [SerializeField] GameObject baseDiamonds;
    [Space]
    [SerializeField] PurchaseItem[] purchaseItems;
    [Space]
    [SerializeField] GameObject restorePopup;
    [SerializeField] GameObject restoreFailPopup;
    [Space]
    [SerializeField] Animation anim;

    private DBController _db;

    public void Init(FirebaseController firebaseController)
    {
        _db = firebaseController.DataBase;
        gameObject.SetActive(false);
    }


    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
        anim.Play();
    }

    public void ShowDiamonds()
    {
        ShowMode(true);
    }

    public void ShowCards()
    {
        ShowMode(false);
    }


    private void ShowMode(bool diamond)
    {
        cards.SetActive(!diamond);
        baseCards.SetActive(!diamond);

        diamonds.SetActive(diamond);
        baseDiamonds.SetActive(diamond);
    }

    public void OnPurchaseComplete(Product product)
    {
        PurchaseItem purchaseItem = purchaseItems.FirstOrDefault(i => i.ID == product.definition.id);
        _db.CardHandler.Value += purchaseItem.CardCount;
        _db.DiamondHandler.Value += purchaseItem.DiamondCount;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        ModalManager.Show("Ошибка!", $"Не удалось получить продукт {product.definition.id}", new ModalButton[] { new ModalButton("Ок") });
    }


    public void OnSuccessRestore(bool success, string message)
    {
        restorePopup.SetActive(false);
        restoreFailPopup.SetActive(true);
        Debug.Log("restore");
        ModalManager.Show("Ошибка!", $"Нет купленых подписок и/или нерастрачиваемых предметов", new ModalButton[] { new ModalButton("Ок") });
    }

    public void OnFetchComplete(Product product)
    {
        //Debug.Log("Fetch");
        try
        {
            PurchaseItem purchaseItem = purchaseItems.FirstOrDefault(i => i.ID == product.definition.id);
            _db.CardHandler.Value += purchaseItem.CardCount;
            _db.DiamondHandler.Value += purchaseItem.DiamondCount;
        }
        catch
        {

        }
    }
}
