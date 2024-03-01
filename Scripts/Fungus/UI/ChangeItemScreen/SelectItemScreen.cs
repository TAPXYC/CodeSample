using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Gravitons.UI.Modal;

public class SelectItemScreen : BaseChangeItemScreen
{
    [Space]
    [SerializeField] Transform diamondCountPosition;
    [SerializeField] Transform diamondCountHidePosition;
    [SerializeField] Transform diamondCountShowPosition;
    [Space]
    [SerializeField] TextMeshProUGUI messageText;
    [Space]
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject blockBuy;
    [Space]
    [SerializeField] Button selectButton;


    public bool CompleteSelect
    {
        get;
        private set;
    }

    private ISelectableInfo[] _items;
    private int _selectedIndex;
    private StoryDiamondCountUI _diamondCountUI;
    private Tweener _showDiamondsCountUI;

    protected override void OnInit()
    {
        selectButton.onClick.AddListener(SelectItem);
        buyButton.onClick.AddListener(BuyItem);
        gameObject.SetActive(false);
        _showDiamondsCountUI = diamondCountPosition.DOLocalMove(diamondCountShowPosition.localPosition, 0.3f)
                                                    .SetAutoKill(false)
                                                    .Pause();
    }




    public void Show(ISelectableInfo[] itemsToSelect, string message, ItemInfo[] startItems = null)
    {
        Init();
        _diamondCountUI = GameManager.Story.UIController.DiamondCountUI;

        CompleteSelect = false;
        messageText.text = message;
        _items = itemsToSelect;
        _selectedIndex = 0;

        gameObject.SetActive(true);
        base.Show(startItems);
    }



    private void SelectItem()
    {
        var selectedItem = _items[_selectedIndex];
        CloseScreen(selectedItem);
    }

    private void BuyItem()
    {
        var selectedItem = _items[_selectedIndex];

        if (selectedItem is ItemInfo)
        {
            var item = (selectedItem as ItemInfo?).Value;

            if (_diamondCountUI.GetDiamondCount() > item.Price)
            {
                _diamondCountUI.ChangeDiamond(-item.Price);
                CloseScreen(selectedItem);
            }
            else
            {
                ModalManager.Show("Недостаточно алмазов", "Перейдите в магазин покупок.", new ModalButton[] { new ModalButton("Ок") });
            }
        }
    }


    private void CloseScreen(ISelectableInfo selectedItem)
    {
        CompleteSelect();
        GameManager.Inst.UIController.Transition(Hide);
    }

    protected override ISelectableInfo GetNextItem()
    {
        _selectedIndex = (_selectedIndex + 1) % _items.Length;
        return _items[_selectedIndex];
    }

    protected override ISelectableInfo GetPrevItem()
    {
        _selectedIndex = _selectedIndex - 1 < 0 ? _items.Length - 1 : _selectedIndex - 1;
        return _items[_selectedIndex];
    }

    protected override ISelectableInfo GetStartItem(ISelectableInfo[] currentItems)
    {
        return _items[_selectedIndex];
    }


    protected override void OnSelectItem(ISelectableInfo selectedItem)
    {
        int price = selectedItem is ItemInfo ? (selectedItem as ItemInfo?).Value.Price : 0;
        bool isForDiamond = price > 0;

        selectButton.gameObject.SetActive(!isForDiamond);
        buyButton.gameObject.SetActive(isForDiamond);

        if (isForDiamond)
        {
            //Проверка на то, куплено ли уже
            if (GameManager.Story.HasAwailableItem(selectedItem))
            {
                selectButton.gameObject.SetActive(true);
                buyButton.gameObject.SetActive(false);
                _diamondCountUI.Hide();
            }
            else
            {
                _diamondCountUI.Show(diamondCountPosition);

                costText.text = price.ToString();
                bool canBuy = _diamondCountUI.GetDiamondCount() > price;
                buyButton.interactable = canBuy;
                blockBuy.SetActive(!canBuy);
            }
        }
        else
        {
            _diamondCountUI.Hide();
        }
    }


    protected override void Hide()
    {
        base.Hide();
        _diamondCountUI.Stash();
        CompleteSelect = true;
    }

    protected override void OnShowScreen(bool show)
    {
        _showDiamondsCountUI.ChangeEndValue(show ? diamondCountShowPosition.localPosition : diamondCountHidePosition.localPosition, true).Restart();
    }
}
