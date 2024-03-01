using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseChangeItemScreen : MonoBehaviour
{
    [Header("Dummies")]
    [SerializeField] MainCharacterController dummyPrefab;
    [SerializeField] Transform baseDummy;
    [SerializeField] int dummyCount = 3;
    [Space]
    [SerializeField] float showDelay = 0;
    [SerializeField] float showMoveDuration = 0.4f;
    [SerializeField] float showFadeDuration = 0.4f;
    [Space]
    [SerializeField] float hideDelay = 0;
    [SerializeField] float hideMoveDuration = 0.4f;
    [SerializeField] float hideFadeDuration = 0.4f;
    [Space]
    [Header("Dummy posotions")]
    [SerializeField] Transform showPosition;
    [SerializeField] Transform centerPosition;
    [SerializeField] Transform hidePosition;
    [Space]
    [Space]
    [Header("Window")]
    [SerializeField] TextMeshProUGUI itemNameText;
    [Space]
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    [Space]
    [SerializeField] Transform baseItemScreen;
    [SerializeField] float downToHide = -510;
    [Space]
    [SerializeField] Button showButton;
    [SerializeField] Button hideButton;

    public ISelectableInfo[] SelectedItems
    {
        get;
        private set;
    }

    public ItemInfo[] CurrentItems
    {
        get;
        private set;
    }

    public CharacterSkinSetting CurrentSkin
    {
        get;
        private set;
    }

    protected MainCharacterController mainCharacter => GameManager.Story.StoryStage.MainCharacter;

    private MainCharacterController[] _dummies;
    private Vector3 _startPosition;
    private Tweener _showTweener;
    private Tween _changeTextTween;
    private string _currentItemName;
    private int _currentDummyIndex;
    private bool _isInited = false;


    void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (!_isInited)
        {
            _isInited = true;

            _startPosition = baseItemScreen.transform.localPosition;

            CreateDummies();

            gameObject.SetActive(false);
            itemNameText.text = "";
            _currentDummyIndex = 0;

            leftButton.onClick.AddListener(() => SetItemInfo(GetPrevItem()));
            rightButton.onClick.AddListener(() => SetItemInfo(GetNextItem()));
            showButton.onClick.AddListener(() => ShowItemSelect(true));
            hideButton.onClick.AddListener(() => ShowItemSelect(false));

            _showTweener = baseItemScreen.DOLocalMove(_startPosition, 0.3f)
                                            .SetAutoKill(false)
                                            .Pause();


            _changeTextTween = DOTween.Sequence().Append(itemNameText.DOFade(0, 0.2f).From(1))
                                                .AppendCallback(() => itemNameText.text = _currentItemName)
                                                .Append(itemNameText.DOFade(1, 0.2f).From(0))
                                                .SetAutoKill(false)
                                                .Pause();
            OnInit();
        }
    }


    private void CreateDummies()
    {
        _dummies = new MainCharacterController[dummyCount];

        for (int i = 0; i < dummyCount; i++)
        {
            var dummy = Instantiate(dummyPrefab, baseDummy);
            dummy.transform.localPosition = Vector3.zero;
            dummy.gameObject.SetActive(true);
            dummy.Init();
            dummy.ForceFade(0);

            _dummies[i] = dummy;
        }
    }


    private void ShowItemSelect(bool show)
    {
        showButton.gameObject.SetActive(!show);
        hideButton.gameObject.SetActive(show);

        OnShowScreen(show);

        _showTweener.ChangeEndValue(show ? _startPosition : _startPosition + Vector3.up*downToHide, true).Restart();
    }


    private void SwitchItem(ItemInfo itemInfo)
    {
        SwitchDummy(d => d.SetItem(itemInfo));
    }


    private void SwitchSkin(CharacterSkinSetting skinSetting)
    {
        SwitchDummy(d => d.SetSkin(skinSetting));
    }



    private void SwitchDummy(Action<MainCharacterController> onChangeDummy)
    {
        var oldDummy = _dummies[_currentDummyIndex];
        _currentDummyIndex = (_currentDummyIndex + 1) % _dummies.Length;
        var newDummy = _dummies[_currentDummyIndex];

        //DebugX.ColorMessage($"Change Item ({itemInfo.Type}) from {oldDummy.GetItemByType(itemInfo.Type).Name} to {itemInfo.Name}", new Color(0.7f, 0.7f, 0.7f));
        MoveDummy(oldDummy, 0f, hidePosition, null, hideMoveDuration, hideFadeDuration, hideDelay);

        MoveDummy(newDummy, 1f, centerPosition, showPosition, showMoveDuration, showFadeDuration, showDelay);

        newDummy.transform.SetAsLastSibling();
        newDummy.ForceFade(0);
        newDummy.SetItems(oldDummy.GetAllSelectedItems());
        onChangeDummy?.Invoke(newDummy);

        SelectedItems = newDummy.GetAllSelectedItems();
        CurrentItems = newDummy.GetSelectedItems();
        CurrentSkin = newDummy.Skin;
    }


    private void MoveDummy(MainCharacterController dummy, float fadeValue, Transform toPosition, Transform fromPosition, float moveDuration, float fadeDuration, float delay)
    {
        DOTween.Kill(dummy.transform);
        var tween = dummy.transform.DOLocalMove(toPosition.localPosition, moveDuration)
                            .SetDelay(delay)
                            .OnStart(() => dummy.Fade(fadeValue, fadeDuration));

        if (fromPosition != null)
            tween.From(fromPosition.localPosition);
    }


    protected void Show(ItemInfo[] startItems = null)
    {
        showButton.gameObject.SetActive(false);
        hideButton.gameObject.SetActive(true);
        baseItemScreen.localPosition = _startPosition;

        CurrentItems = mainCharacter.GetSelectedItems();
        CurrentSkin = mainCharacter.Skin;
        SelectedItems = mainCharacter.GetAllSelectedItems();

        foreach (var dummy in _dummies)
        {
            dummy.ForceFade(0);

            if (!startItems.IsNullOrEmpty())
                dummy.SetItems(startItems);
            else
                dummy.SetItems(SelectedItems);
        }

        var currentItem = GetStartItem(SelectedItems);

        itemNameText.alpha = 1;
        itemNameText.text = currentItem.GetName();

        var startDummy = _dummies[_currentDummyIndex];

        if (currentItem is ItemInfo)
            startDummy.SetItem((currentItem as ItemInfo?).Value);

        if (currentItem is CharacterSkinSetting)
            startDummy.SetSkin(currentItem as CharacterSkinSetting);

        DOTween.Kill(startDummy.transform);
        startDummy.ForceFade(1);
        startDummy.transform.SetAsLastSibling();
        startDummy.transform.localPosition = centerPosition.localPosition;

        CurrentItems = startDummy.GetSelectedItems();
        CurrentSkin = startDummy.Skin;
        SelectedItems = startDummy.GetAllSelectedItems();

        OnSelectItem(currentItem);
    }


    protected void SetItemInfo(ISelectableInfo itemInfo)
    {
        _currentItemName = itemInfo.GetName();
        _changeTextTween.Restart();

        if (itemInfo is ItemInfo)
            SwitchItem((itemInfo as ItemInfo?).Value);

        if (itemInfo is CharacterSkinSetting)
            SwitchSkin(itemInfo as CharacterSkinSetting);

        OnSelectItem(itemInfo);
    }


    protected virtual void CompleteSelect()
    {
        mainCharacter.SetItems(SelectedItems);
        GameManager.Story.SetItems(mainCharacter.GetAllSelectedItems());
    }

    protected virtual void Hide()
    {
        gameObject.SetActive(false);
    }



    protected abstract void OnInit();
    protected abstract void OnShowScreen(bool show);
    protected abstract ISelectableInfo GetStartItem(ISelectableInfo[] selectedItems);
    protected abstract ISelectableInfo GetPrevItem();
    protected abstract ISelectableInfo GetNextItem();
    protected abstract void OnSelectItem(ISelectableInfo itemInfo);
}
