using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemGroup : MonoBehaviour
{
    [SerializeField] Image tabIcon;
    [SerializeField] Button tabButton;
    [SerializeField] Transform selectedPosition;
    [SerializeField] Transform unselectedPosition;

    public event Action<ItemGroup> OnSelect;


    public SelectableItemType ItemType => Items[0].GetItemType();

    public bool IsSelected
    {
        get;
        private set;
    }
    public ISelectableInfo[] Items
    {
        get;
        private set;
    }

    private Tweener _moveTweener;
    private int _selectedIndex;

    public void Init(Sprite tabSprite, ISelectableInfo[] items, bool showTab, bool isSelected)
    {
        tabIcon.sprite = tabSprite;
        Items = items;
        tabButton.gameObject.SetActive(showTab);

        if (showTab)
        {
            IsSelected = isSelected;

            tabButton.transform.localPosition = isSelected ? selectedPosition.localPosition : unselectedPosition.localPosition;

            _moveTweener = tabButton.transform.DOLocalMove(unselectedPosition.localPosition, 0.5f)
                                                .SetAutoKill(false)
                                                .Pause();

            tabButton.onClick.AddListener(() => SetSelect(true));
        }
    }



    public void SetStartItem(ISelectableInfo itemInfo)
    {
        if (itemInfo != null && itemInfo.GetIsCreated())
        {
            _selectedIndex = IndexOf(Items, itemInfo);
        }
    }


    public ISelectableInfo GetCurrentItem()
    {
        return Items[_selectedIndex];
    }

    public ISelectableInfo GetNextItem()
    {
        _selectedIndex = (_selectedIndex + 1) % Items.Length;
        return Items[_selectedIndex];
    }

    public ISelectableInfo GetPrevItem()
    {
        _selectedIndex = _selectedIndex - 1 < 0 ? Items.Length - 1 : _selectedIndex - 1;
        return Items[_selectedIndex];
    }

    private int IndexOf(ISelectableInfo[] elements, ISelectableInfo item)
    {
        string findedID = item.GetID();
        int index = 0;

        for (; index < elements.Length; index++)
        {
            if (elements[index].GetID() == findedID)
            {
                break;
            }
        }

        return index;
    }



    public void SetSelect(bool selected)
    {
        if (_moveTweener != null && IsSelected != selected)
        {
            IsSelected = selected;
            _moveTweener.ChangeEndValue(selected ? selectedPosition.localPosition : unselectedPosition.localPosition, true).Restart();

            if (IsSelected)
            {
                OnSelect?.Invoke(this);
            }
        }
    }
}
