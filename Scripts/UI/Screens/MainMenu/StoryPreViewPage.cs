using System;
using Gravitons.UI.Modal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryPreViewPage : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image storyName;
    [SerializeField] TextMeshProUGUI storySummary;
    [SerializeField] TextMeshProUGUI seasonNum;
    [SerializeField] TextMeshProUGUI seriaNum;
    [Space]
    [SerializeField] Button freePlayButton;
    [SerializeField] Button cardPlayButton;
    [SerializeField] Button commingSoonButton;
    [Space]
    [SerializeField] Animation anim;
    [SerializeField] GameObject restartWindow;
    [SerializeField] GameObject loadingBlock;
    [SerializeField] Button seriesRestartButton;
    [SerializeField] Button seasonRestartButton;

    public event Action<StoryDataInfo> OnSelectStory;

    private StoryDataInfo _currentStoryData;
    private IntegerHandler _cardHandler;
    private FirebaseController _firebaseController;
    private bool _canStart;

    public void Init(FirebaseController firebaseController)
    {
        _firebaseController = firebaseController;
        gameObject.SetActive(false);
        loadingBlock.SetActive(false);
        freePlayButton.onClick.AddListener(StartSeries);
        cardPlayButton.onClick.AddListener(BuySeria);
        commingSoonButton.onClick.AddListener(CommingSoon);
        seriesRestartButton.onClick.AddListener(SeriesRestart);
        seasonRestartButton.onClick.AddListener(SeasonRestart);
    }

    private void CommingSoon()
    {
        ModalManager.Show("В разработке", "Подождите пока создадут истории", new ModalButton[] { new ModalButton("Ок") });
    }

    private void SeasonRestart()
    {
        var cardHandler = _firebaseController.DataBase.CardHandler;

        if (cardHandler.Value > 0)
        {
            ModalManager.Show("Перезапустить сезон", "Все данные будут утеряны", new ModalButton[] {
                                                                                        new ModalButton("Да", () =>
                                                                                        {
                                                                                            cardHandler.Value--;
                                                                                            loadingBlock.SetActive(true);
                                                                                            _currentStoryData.ResetSeason(() => { Show(_currentStoryData); });
                                                                                        }),
                                                                                        new ModalButton("Нет") });
        }
        else
        {
            ModalManager.Show("Недостаточно карт!", $"Подождите {GameManager.Inst.UIController.CardHandler.RemainingText} пока восполнится", new ModalButton[] { new ModalButton("Ок") });
        }
    }

    private void SeriesRestart()
    {
        var cardHandler = _firebaseController.DataBase.CardHandler;

        if (cardHandler.Value > 0)
        {
            ModalManager.Show("Перезапустить серию", "Все данные будут утеряны", new ModalButton[] {
                                                                                        new ModalButton("Да", () =>
                                                                                        {
                                                                                            cardHandler.Value--;
                                                                                            loadingBlock.SetActive(true);
                                                                                            _currentStoryData.ResetSeries(() => { Show(_currentStoryData); });
                                                                                        }),
                                                                                        new ModalButton("Нет") });
        }
        else
        {
            Debug.LogError(GameManager.Inst.UIController.CardHandler.RemainingSeconds);
            ModalManager.Show("Недостаточно карт!", $"Подождите {GameManager.Inst.UIController.CardHandler.RemainingText} пока восполнится", new ModalButton[] { new ModalButton("Ок") });
        }
    }

    private void BuySeria()
    {
        if (_cardHandler.Value > 0)
        {
            _cardHandler.Value--;
            _currentStoryData.SetUnlocked(true);
            SetUnlockedMode(true);
            StartSeries();
        }
        else
        {
            ModalManager.Show("Недостаточно карт!", $"Подождите {GameManager.Inst.UIController.CardHandler.RemainingText} пока восполнится", new ModalButton[] { new ModalButton("Ок") });
        }
    }

    private void StartSeries()
    {
        if (_canStart)
        {
            _canStart = false;
            OnSelectStory?.Invoke(_currentStoryData);
        }
    }

    public void Show(StoryDataInfo storyData)
    {
        loadingBlock.SetActive(false);
        _canStart = true;
        _cardHandler = _firebaseController.DataBase.CardHandler;
        _currentStoryData = storyData;
        background.sprite = _currentStoryData.Background;
        storyName.sprite = _currentStoryData.Header;
        storySummary.text = _currentStoryData.Decription;
        seasonNum.text = $"{_currentStoryData.CurrentSeason + 1} сезон";
        seriaNum.text = $"Серия {_currentStoryData.SeriesNumber + 1}/{_currentStoryData.SeasonSeriesCount}";

        commingSoonButton.gameObject.SetActive(storyData.CompleteStory);

        if (!storyData.CompleteStory)
        {
            SetUnlockedMode(storyData.Unlocked);
        }
        else
        {
            freePlayButton.gameObject.SetActive(false);
            cardPlayButton.gameObject.SetActive(false);
        }

        restartWindow.SetActive(false);
        gameObject.SetActive(true);
        anim.Play();
    }

    public void ForceHide()
    {
        gameObject.SetActive(false);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OpenRestartWindow()
    {
        restartWindow.SetActive(true);
    }

    public void CloseRestartWindow()
    {
        restartWindow.SetActive(false);
    }


    private void SetUnlockedMode(bool unlocked)
    {
        freePlayButton.gameObject.SetActive(unlocked);
        cardPlayButton.gameObject.SetActive(!unlocked);
    }
}
