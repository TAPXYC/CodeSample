using EasyTransition;
using UnityEngine;
using System;
using Gravitons.UI.Modal;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] LogoLoad transitionLogo;
    [SerializeField] GameObject baseUI;
    [SerializeField] RectTransform baseContent;
    [SerializeField] GameObject mainBackground;
    [Space]
    [SerializeField] TransitionSettings transitionSettings;
    [SerializeField] StoryTransitionScreen storyTransitionScreen;
    [Space]
    [SerializeField] MainMenu mainMenu;
    [SerializeField] SettingsMenu settingMenu;
    [SerializeField] SignInWindow signInMenu;
    [SerializeField] StoreMenu storeMenu;
    [SerializeField] BonusesMenu bonusMenu;
    [Space]
    [SerializeField] MenuDiamondHandler diamondHandler;
    [SerializeField] MenuCardHandler cardHandler;
    [Space]
    [SerializeField] ClampRewardPopup clampReward;

    public MainMenu MainMenu => mainMenu;
    public ClampRewardPopup ClampReward => clampReward;
    public MenuCardHandler CardHandler => cardHandler;

    private TransitionManager _transitionManager;
    private GameObject _lastTransition;
    private FirebaseController _firebaseController;

    public void Init(FirebaseController firebaseController, StoryDataInfo[] storyInfos)
    {
        _firebaseController = firebaseController;
        gameObject.SetActive(true);

        clampReward.Init();
        _transitionManager = TransitionManager.Instance();

        mainMenu.Init(storyInfos, firebaseController);

        settingMenu.Init(firebaseController);
        settingMenu.OnShowSignIn += signInMenu.ShowMenu;

        storeMenu.Init(firebaseController);
        signInMenu.Init(firebaseController);

        storyTransitionScreen.Init();
        mainBackground.SetActive(true);

        diamondHandler.OnClick += OpenDiamondStore;
        cardHandler.OnClick += OpenCardsStore;
    }


   


    public void GameLoaded(DailyRewardController dailyRewardController)
    {
        diamondHandler.Init();
        cardHandler.Init();
        bonusMenu.Init(dailyRewardController);

        // показ окна регистрации в первый раз
        if (!_firebaseController.Auth.IsLoggin && GameManager.IsFirstStart)
        {
            ModalManager.Show("Авторизация", "Зарегистрируйтесь чтобы не потерять прогресс", new[]
                                                                                            {
                                                                                                new ModalButton("Регистрация", () =>  signInMenu.ShowMenu()),
                                                                                                new ModalButton("Нет, спасибо", () => ModalManager.Show("Авторизация",
                                                                                                                "Вы можете зарегистрироваться в любой момент в настройках игры",
                                                                                                                new [] {new ModalButton("Закрыть")}))});
        }
    }

    public void ShowLogo(bool show)
    {
        transitionLogo.StartScreen(show);
    }


    public void ShowLogo(Func<TMP_Text, Action<float>, IEnumerator> proccess)
    {
        transitionLogo.StartScreen(proccess);
    }



    public void ShowStoryLoadingScreen(StoryDataInfo storyData)
    {
        storyTransitionScreen.Show(storyData);
    }


    public void SetStoryLoadingProgress(Func<TMP_Text, IEnumerator> proccess)
    {
        storyTransitionScreen.SetLoad(proccess);
    }

    public void HideStoryLoadingScreen()
    {
        storyTransitionScreen.Hide();
    }

    public void Show(bool showLastStory)
    {
        settingMenu.gameObject.SetActive(false);

        storeMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        mainBackground.SetActive(true);
        mainMenu.UpdateWindow();

        if (showLastStory)
            mainMenu.ShowLastStory();

        baseUI.SetActive(true);
    }

    public void Hide()
    {
        baseUI.SetActive(false);
        mainBackground.SetActive(false);
    }


    public void Transition(int sceneIndex, Action action)
    {
        if (_lastTransition != null)
        {
            Destroy(_lastTransition);
        }

        _transitionManager.Transition(sceneIndex, transitionSettings, null, action, null);
    }

    public void Transition(Action action, float cutDelay = 0.1f)
    {
        if (_lastTransition != null)
        {
            Destroy(_lastTransition);
        }

        _transitionManager.Transition(transitionSettings, cutDelay, null, action, null);
    }

    public void Transition(Action cutAction, Action completeAction, float cutDelay = 0.1f)
    {
        if (_lastTransition != null)
        {
            Destroy(_lastTransition);
        }

        _transitionManager.Transition(transitionSettings, cutDelay, null, cutAction, completeAction);
    }



    public void OpenBonusMenu()
    {
        bonusMenu.Show(true);
    }

    public void OpenSettings()
    {
        settingMenu.ShowMenu();
    }

    public void OpenDiamondStore()
    {
        OpenStore();
        storeMenu.ShowDiamonds();
    }

    public void OpenCardsStore()
    {
        OpenStore();
        storeMenu.ShowCards();
    }

    private void OpenStore()
    {
        storeMenu.ShowMenu();
    }
}
