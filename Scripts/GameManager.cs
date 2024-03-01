using System.Collections;
using UnityEngine;
using System;
using DG.Tweening;
using Fungus;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;


public enum CurrencyType
{
    Diamond,
    Cards
}


public class GameManager : MonoBehaviour
{
    [SerializeField] AudioClip menuMusic;
    [SerializeField] bool showDebug;
    [Space]
    [SerializeField] AssetReferenceT<StoriesCollectList> storiesAsset;
    [Space]
    [SerializeField] UIController uIController;
    [Space]
    [SerializeField] FirebaseController firebaseController;
    [SerializeField] DailyRewardController dailyReward;
    [SerializeField] MaxSDKController applovinController;
    [SerializeField] NativeNotificationController notificationController;

    public event Action OnBeginLoading;
    public event Action OnLoaded;

    public static GameManager Inst;
    public static bool ShowDebug => Inst.showDebug;
    public static StoryController Story => Inst._storyScene == null ? null : Inst._storyScene.Story;
    public static MaxSDKController ADS => Inst.applovinController;
    public static NativeNotificationController NotificationController => Inst.notificationController;

    public UIController UIController => uIController;
    public FirebaseController FirebaseController => firebaseController;

    public static bool IsFirstStart
    {
        get;
        private set;
    }

    private StoryDataInfo _currentStoryData;
    private StorySceneController _storyScene;
    private StoryController _storyPrefab;
    private ItemCollection _storyItems;
    private StoriesCollectList _storiesList;

    void Awake()
    {
        Inst = this;

        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(gameObject);
        uIController.gameObject.SetActive(true);
        uIController.ShowLogo(LoadCoroutine);

        IsFirstStart = PlayerPrefs.GetInt("IsFirstStart", 1) == 1;

        if (IsFirstStart)
            PlayerPrefs.SetInt("IsFirstStart", 0);

        firebaseController.Init();
        applovinController.Init();
    }




    private IEnumerator LoadCoroutine(TMP_Text tb, Action<float> setFill)
    {
        if (_storiesList == null)
        {
            bool loadSuccess;

            do
            {
                /////////ЗАГРУЗКА СПИСКА ИСТОРИЙ////////////
                loadSuccess = true;
                AsyncOperationHandle<StoriesCollectList> asyncOperationHandle = storiesAsset.LoadAssetAsync();

                while (!asyncOperationHandle.IsDone)
                {
                    var status = asyncOperationHandle.GetDownloadStatus();
                    float progress = status.Percent;

                    setFill(progress);

                    tb.text = $"Загрузка данных {(int)(progress * 100)} %";
                    yield return new WaitForSeconds(0.5f);
                }

                if (asyncOperationHandle.OperationException != null)
                {
                    storiesAsset.ReleaseAsset();
                    Debug.LogError("Ошибка загрузки списка историй");
                    loadSuccess = false;
                }

                if (loadSuccess)
                {
                    _storiesList = asyncOperationHandle.Result;

                    /////////ЗАГРУЗКА ДАННЫХ КАЖДОЙ ИСТОРИИ////////////

                    AsyncOperationHandle<StoryDataInfo>[] asyncStoryDataInfoHandles = _storiesList.GetStoriesDatas();

                    if (!asyncStoryDataInfoHandles.IsNullOrEmpty())
                    {
                        foreach (var ao in asyncStoryDataInfoHandles)
                        {
                            while (!ao.IsDone)
                            {
                                var status = ao.GetDownloadStatus();
                                float progress = status.Percent;

                                setFill(progress);

                                tb.text = $"Загрузка обновлений {(int)(progress * 100)} %";
                                yield return new WaitForSeconds(0.5f);
                            }

                            if (ao.OperationException != null)
                            {
                                _storiesList.ReleaseAssets();
                                Debug.LogError("Ошибка загрузки данных историии " + asyncStoryDataInfoHandles.ToList().IndexOf(ao));
                                loadSuccess = false;
                                break;
                            }
                        }
                    }
                }


                if (!loadSuccess)
                {
                    setFill(0f);
                    for (int timeout = 5; timeout >= 0; timeout--)
                    {
                        tb.text = $"Ошибка соединения с сервером!\nПовторное подключение через {timeout}";
                        yield return new WaitForSeconds(1f);
                    }
                }

            } while (!loadSuccess);
        }

        yield return new WaitForSeconds(0.5f);

        /////////////////ИНИЦИАЛИЗАЦИЯ ДАННЫХ////////////////////

        var storyInfos = _storiesList.StoryInfos;

        uIController.Init(firebaseController, storyInfos);
        uIController.Show(false);
        uIController.MainMenu.OnSelectStory += LoadStory;

        foreach (var storyInfo in storyInfos)
        {
            storyInfo.Init(firebaseController.DataBase);
        }

        firebaseController.DataBase.UpdateUserData();

        setFill(1f);
        tb.text = $"Завершение...";

        ///////////////////ЗАВЕРШЕНИЕ ЗАГРУЗКИ ДАННЫХ И ЗАПУСК ПРИЛОЖЕНИЯ//////////////////
        notificationController.Init();
        notificationController.ClearNotification();

        yield return new WaitUntil(() => firebaseController.Auth.IsLoaded);
        yield return new WaitUntil(() => firebaseController.DataBase.IsLoaded);

        var musicManager = FungusManager.Instance.MusicManager;
        musicManager.PlayMusic(menuMusic, true, 0.5f, 0);

        dailyReward.Init();
        uIController.GameLoaded(dailyReward);

        yield return new WaitForSeconds(0.5f);

        uIController.Transition(() =>
        {
            uIController.ShowLogo(false);
            OnLoaded?.Invoke();
        });
    }



    private void LoadStory(StoryDataInfo storyData)
    {
        _currentStoryData = storyData;

        uIController.Transition(() =>
        {
            CloseCurrentStory();
            uIController.Hide();
            uIController.ShowStoryLoadingScreen(_currentStoryData);
            uIController.SetStoryLoadingProgress(tb => LoadStoryDataCoroutine(_currentStoryData, tb, success =>
            {
                if (success)
                    SceneManager.LoadScene(1);
                else
                    ReturnToMainMenu();
            }));
        });
    }


    private IEnumerator LoadStoryDataCoroutine(StoryDataInfo storyDataInfo, TMP_Text tb, Action<bool> onComplete)
    {
        bool loadSuccess = true;

        AsyncOperationHandle<ItemCollection>? itemsLoad = storyDataInfo.LoadItems();
        AsyncOperationHandle<GameObject>? prefabGOLoad = storyDataInfo.CurrentSeriesInfo.LoadStoryPrefab();

        if (itemsLoad != null)
        {
            while (!itemsLoad.Value.IsDone)
            {
                var status = itemsLoad.Value.GetDownloadStatus();
                float progress = status.Percent;

                tb.text = $"Загрузка данных {(int)(progress * 100)} %";
                yield return new WaitForSeconds(0.5f);
            }

            if (itemsLoad.Value.OperationException != null)
            {
                storyDataInfo.ReleaseItems();
                loadSuccess = false;
            }
        }

        if (prefabGOLoad != null)
        {
            while (!prefabGOLoad.Value.IsDone)
            {
                var status = prefabGOLoad.Value.GetDownloadStatus();
                float progress = status.Percent;

                tb.text = $"Загрузка истории {(int)(progress * 100)} %";
                yield return new WaitForSeconds(0.5f);
            }

            if (prefabGOLoad.Value.OperationException != null)
            {
                loadSuccess = false;
                storyDataInfo.CurrentSeriesInfo.ReleaseStoryPrefab();
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (loadSuccess)
        {
            tb.text = "Создание окружения...";
            _storyItems = storyDataInfo.Items;
            _storyPrefab = storyDataInfo.CurrentSeriesInfo.StoryGO.GetComponent<StoryController>();

            yield return new WaitForSeconds(1f);
            onComplete?.Invoke(loadSuccess);
        }
        else
        {
            tb.text = $"Ошибка соединения с сервером!";
            yield return new WaitForSeconds(1f);
            onComplete?.Invoke(loadSuccess);
        }
    }



    public void SetStoryScene(StorySceneController storySceneController)
    {
        _storyScene = storySceneController;
        _storyScene.LoadStory(_currentStoryData, _storyItems, _storyPrefab, StoryLoaded);
    }



    private void StoryLoaded()
    {
        Story.OnCompleteStory += StoryComplete;
        DOTween.Sequence().AppendInterval(3f)
                            .AppendCallback(() => uIController.Transition(() => uIController.HideStoryLoadingScreen()));
    }



    private void StoryComplete(bool successComplete)
    {
        var musicManager = FungusManager.Instance.MusicManager;
        musicManager.PlayMusic(menuMusic, true, 0.5f, 0);

        if (successComplete)
        {
            _currentStoryData.NextStory();
        }

        ReturnToMainMenu();
    }


    private void ReturnToMainMenu()
    {
        uIController.Transition(() =>
        {
            uIController.ShowLogo(true);
            uIController.HideStoryLoadingScreen();
            CloseCurrentStory();
            uIController.Show(true);
        }, () =>
        {
            uIController.Transition(() => uIController.ShowLogo(false));
        }, 0.2f);
    }



    private void CloseCurrentStory()
    {
        if (Story != null)
        {
            _storyScene.Clear();
            _storyScene = null;
        }
    }
}
