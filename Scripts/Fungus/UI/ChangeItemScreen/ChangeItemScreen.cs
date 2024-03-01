using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChangeItemScreen : BaseChangeItemScreen
{
    [Space]
    [SerializeField] GameObject baseVisual;
    [Space]
    [SerializeField] Button completeButton;
    [Space]
    [Space]
    [SerializeField] ItemGroup itemGroupPrefab;
    [SerializeField] Transform baseItemGroup;
    [Space]
    [SerializeField] ItemTypeIcon[] itemTypeIcons;

    [Serializable]
    private struct ItemTypeIcon
    {
        public SelectableItemType ItemType;
        public Sprite Icon;
    }

    private List<ItemGroup> _itemGroups;
    private ItemGroup _selectedGroup;


    protected override void OnInit()
    {
        _itemGroups = new List<ItemGroup>();
        completeButton.onClick.AddListener(Close);
        gameObject.SetActive(false);
    }



    public void Show()
    {
        Init();
        var awailableItems = GameManager.Story.GetAwailableItems();

        var GroupedItems = awailableItems.GroupBy(ii => ii.GetItemType()).ToDictionary(g => g.Key, g => g.ToList());
        bool showTabs = GroupedItems.Count > 1;

        if (GroupedItems.Count == 0)
        {
            Debug.LogError("No items");
            return;
        }

        foreach (var item in GroupedItems)
        {
            bool noSelectedGroup = _selectedGroup == null;
            Sprite groupSprite = itemTypeIcons.First(ii => ii.ItemType == item.Key).Icon;

            var group = Instantiate(itemGroupPrefab, baseItemGroup);
            group.Init(groupSprite, item.Value.ToArray(), showTabs, noSelectedGroup);
            group.name += $" ({item.Key})";

            if (noSelectedGroup)
                _selectedGroup = group;

            if (showTabs)
                group.OnSelect += ChangeTab;

            _itemGroups.Add(group);
        }

        gameObject.SetActive(true);
        baseVisual.SetActive(false);
        base.Show();

        GameManager.Inst.UIController.Transition(() => baseVisual.SetActive(true));
    }



    private void ChangeTab(ItemGroup selectedGroup)
    {
        _selectedGroup = selectedGroup;
        _selectedGroup.SetStartItem(GetItemByType(SelectedItems, _selectedGroup.ItemType));

        SetItemInfo(_selectedGroup.GetCurrentItem());

        foreach (var group in _itemGroups.Where(g => g != _selectedGroup))
        {
            group.SetSelect(false);
        }
    }


    private void Close()
    {
        CompleteSelect();
        GameManager.Inst.UIController.Transition(Hide);
    }

    private void ClearGroups()
    {
        foreach (var group in _itemGroups)
        {
            group.OnSelect -= ChangeTab;
            Destroy(group.gameObject);
        }
        _selectedGroup = null;
        _itemGroups.Clear();
    }

    protected override ISelectableInfo GetStartItem(ISelectableInfo[] selectedItems)
    {
        foreach (var group in _itemGroups)
        {
            var groupHandledType = group.ItemType;
            group.SetStartItem(GetItemByType(selectedItems, groupHandledType));
        }

        return _selectedGroup.GetCurrentItem();
    }


    protected override ISelectableInfo GetPrevItem()
    {
        return _selectedGroup.GetPrevItem();
    }

    protected override ISelectableInfo GetNextItem()
    {
        return _selectedGroup.GetNextItem();
    }


    protected override void Hide()
    {
        ClearGroups();
        gameObject.SetActive(false);
    }


    private ISelectableInfo GetItemByType(ISelectableInfo[] items, SelectableItemType type)
    {
        return items.FirstOrDefault(i => i.GetItemType() == type);
    }

    protected override void OnSelectItem(ISelectableInfo itemInfo) { }

    protected override void OnShowScreen(bool show) { }
}
