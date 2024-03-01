using System;
using Fungus;
using UnityEngine;

public class StoryUIController : MonoBehaviour
{
    [SerializeField] RectTransform baseContent;
    [Space]
    [SerializeField] AdaptiveMenuDialog menuDialog;
    [Space]
    [SerializeField] DropdownSayDialog leftDialog;
    [SerializeField] DropdownSayDialog rightDialog;
    [SerializeField] DropdownSayDialog centerDialog;
    [Space]
    [SerializeField] SimpleMessageDialog parameterDialog;
    [SerializeField] SimpleMessageDialog upperDialog;
    [Space]
    [SerializeField] InputNameWindow inputNameWindow;
    [SerializeField] ChangeItemScreen changeItemScreen;
    [SerializeField] SelectItemScreen selectItemScreen;
    [Space]
    [SerializeField] StoryDiamondCountUI diamondCountUI;
    [Space]
    [SerializeField] StoryMenuController storyMenu;
    [SerializeField] StatsScreen statsScreen;
    [SerializeField] EndScreen endScreen;


    public event Action OnMainMenu;

    public AdaptiveMenuDialog MenuDialog => menuDialog;
    public DropdownSayDialog LeftDialog => leftDialog;
    public DropdownSayDialog CenterDialog => centerDialog;
    public DropdownSayDialog RightDialog => rightDialog;

    public SimpleMessageDialog ParameterDialog => parameterDialog;
    public SimpleMessageDialog UpperDialog => upperDialog;

    public InputNameWindow InputNameWindow => inputNameWindow;
    public ChangeItemScreen ChangeItemScreen => changeItemScreen;
    public StoryDiamondCountUI DiamondCountUI => diamondCountUI;
    public SelectItemScreen SelectItemScreen => selectItemScreen;
    public EndScreen EndScreen => endScreen;

    private float _lastBannerHeight;

    public void Init(Sprite endScreenBackground)
    {
        menuDialog.Init();

        leftDialog.Init();
        rightDialog.Init();
        centerDialog.Init();

        parameterDialog.Init();
        upperDialog.Init();

        diamondCountUI.Init();

        statsScreen.Init();
        endScreen.Init();
        endScreen.SetBackground(endScreenBackground);

        storyMenu.Init();

        changeItemScreen.Init();
        selectItemScreen.Init();

        Fungus.MenuDialog.ActiveMenuDialog = menuDialog;

        if (Fungus.SayDialog.ActiveSayDialog == null)
            Fungus.SayDialog.ActiveSayDialog = centerDialog;

        storyMenu.OnMainMenuClick += MainMenuClick;
        storyMenu.OnShowItemsChangeScreenClick += ShowChangeItemsScreen;
        storyMenu.OnShowStatsClick += ShowStatScreen;
    }


    void Update()
    {
        float bannerHeight = GameManager.ADS.BannerHeight;

        if (!_lastBannerHeight.IsEqFloat(bannerHeight))
        {
            _lastBannerHeight = bannerHeight;

            if (baseContent != null)
                baseContent.SetBottom(_lastBannerHeight);
                else
                Debug.LogError("На этом уровне не задан baseContent!");
        }
    }


    private void MainMenuClick()
    {
        OnMainMenu?.Invoke();
    }

    private void ShowStatScreen()
    {
        statsScreen.Show();
    }

    private void ShowChangeItemsScreen()
    {
        changeItemScreen.Show();
    }
}
