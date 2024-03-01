using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] StoryPreViewPage storyPreviewPage;
    [Space]
    [SerializeField] Transform baseStoryHandlers;
    [SerializeField] StoryUIHandler storyUIHandlerPrefab;
    [SerializeField] ScrollRect scroll;
    [Space]
    [Header("ADS")]
    [SerializeField] Button adButton;
    [SerializeField] GameObject baseTimer;
    [SerializeField] WatchAd watchAd;
    [SerializeField] AddRewardPopup addRewardPopup;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] int adsCooldown = 180;
    [SerializeField] int adsReward = 2;

    public event Action<StoryDataInfo> OnSelectStory;

    private StoryDataInfo _lastStoryInfo;
    private const string _adsRewardSaveKey = "lastRewardTime";

    public void Init(StoryDataInfo[] stories, FirebaseController firebaseController)
    {
        foreach (var story in stories)
        {
            StoryUIHandler storyUIHandler = Instantiate(storyUIHandlerPrefab, baseStoryHandlers);
            storyUIHandler.Init(story);
            storyUIHandler.OnStoryClick += ShowStoryInfo;
        }
        storyPreviewPage.Init(firebaseController);
        storyPreviewPage.OnSelectStory += PlayStory;

        watchAd.gameObject.SetActive(false);
        addRewardPopup.gameObject.SetActive(false);
        adButton.onClick.AddListener(ADS_Click);

        if (PlayerPrefs.HasKey(_adsRewardSaveKey))
        {
            DateTime lastAddTime = new DateTime(long.Parse(PlayerPrefs.GetString(_adsRewardSaveKey)));

            int completedSeconds = (int)(DateTime.UtcNow - lastAddTime).TotalSeconds;
            DebugX.ColorMessage($"Последний ревард {completedSeconds} секунд назад", Color.magenta);

            if (completedSeconds >= adsCooldown)
            {
                SetADSTimer(false);
            }
            else
            {
                StartTimer(adsCooldown - completedSeconds);
            }
        }
        else
            SetADSTimer(false);

        UpdateWindow();
    }

    private void ADS_Click()
    {
        watchAd.ShowWindow(yes =>
        {
            if (yes)
            {
                GameManager.ADS.TryShowReward(success =>
                {
                    if (success)
                    {
                        addRewardPopup.Show(adsReward, () =>
                        {
                            StartTimer(adsCooldown);

                            PlayerPrefs.SetString(_adsRewardSaveKey, DateTime.UtcNow.Ticks.ToString());
                            GameManager.Inst.FirebaseController.DataBase.DiamondHandler.Value += adsReward;

                            GameManager.NotificationController.SendNotification("Время пришло!", "Пора смотреть рекламу и собирать алмазы!", DateTime.UtcNow.AddSeconds(adsCooldown));
                        });
                    }
                });
            }
        });
    }


    private void StartTimer(int duration)
    {
        int time = duration;
        timerText.text = new DateTime().AddSeconds(time).ToString("H:mm:ss") + $"  +{adsReward}";
        time--;

        DOTween.Sequence().AppendInterval(1)
                    .SetLoops(adsCooldown)
                    .OnStepComplete(() =>
                    {
                        timerText.text = new DateTime().AddSeconds(time).ToString("H:mm:ss") + $"  +{adsReward}";
                        time--;
                    })
                    .OnComplete(() => SetADSTimer(false));

        SetADSTimer(true);
    }

    private void SetADSTimer(bool enableTimer)
    {
        adButton.interactable = !enableTimer;
        baseTimer.SetActive(enableTimer);
    }


    public void UpdateWindow()
    {
        storyPreviewPage.ForceHide();
        scroll.normalizedPosition = new Vector2(0, 1);
    }


    public void ShowLastStory()
    {
        ShowStoryInfo(_lastStoryInfo);
    }


    private void PlayStory(StoryDataInfo info)
    {
        _lastStoryInfo = info;
        SelectStory(info);
    }


    private void ShowStoryInfo(StoryDataInfo info)
    {
        storyPreviewPage.Show(info);
    }


    private void SelectStory(StoryDataInfo story)
    {
        OnSelectStory?.Invoke(story);
    }



    /// <summary>
    /// Отрисовка кнопки показа дебаг консоли аппловина
    /// </summary>
    void OnGUI()
    {
        if (GameManager.ShowDebug)
        {
            if (GUI.Button(new Rect(Camera.main.pixelWidth - 170 - 120, 10, 110, 40), "100 cards"))
                GameManager.Inst.FirebaseController.DataBase.CardHandler.Value += 100;

            if (GUI.Button(new Rect(Camera.main.pixelWidth - 170 - 120 - 120, 10, 110, 40), "1000 diamonds"))
                GameManager.Inst.FirebaseController.DataBase.DiamondHandler.Value += 1000;
        }
    }
}
