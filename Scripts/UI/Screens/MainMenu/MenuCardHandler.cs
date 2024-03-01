using System;
using System.Collections;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuCardHandler : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] int reloadDuration = 10800;
    [SerializeField] TextMeshProUGUI diamondsCount;
    [SerializeField] Button clickButton;
    [Space]
    [SerializeField] TextMeshProUGUI timerText;

    public event Action OnClick;
    public int RemainingSeconds => (int)(_reloadTween.Duration() - _reloadTween.Elapsed());
    public string RemainingText => new DateTime().AddSeconds(RemainingSeconds).ToString("H:mm:ss");

    private IntegerHandler _cardHandler;
    private Tweener _counterTweener;
    private Tween _textPunchTween;
    private Tween _reloadTween;


    public void Init()
    {
        _cardHandler = GameManager.Inst.FirebaseController.DataBase.CardHandler;
        int completedSeconds = 0;
        _reloadTween = null;

        if (PlayerPrefs.HasKey("lastCardAddTime"))
        {
            DateTime lastAddTime = new DateTime(long.Parse(PlayerPrefs.GetString("lastCardAddTime")));

            completedSeconds = (int)(DateTime.UtcNow - lastAddTime).TotalSeconds;
            DebugX.ColorMessage($"Последняя карта добавлена {completedSeconds} секунд назад", Color.yellow);

            while (_cardHandler.Value < max && completedSeconds > reloadDuration)
            {
                completedSeconds -= (int)reloadDuration;
                _cardHandler.Value++;
            }
        }

        _cardHandler.OnChangeValueDirection += ChangeValue;
        clickButton.onClick.AddListener(() => OnClick?.Invoke());

        int currentVisibleValue = _cardHandler.Value;
        diamondsCount.text = currentVisibleValue.ToString();

        _textPunchTween = diamondsCount.transform.DOPunchScale(new Vector3(0.4f, 0.2f, 0), 0.3f, 10, 1)
                                                    .SetAutoKill(false)
                                                    .Pause();

        _counterTweener = DOTween.To(() => currentVisibleValue, value =>
                                    {
                                        currentVisibleValue = value;
                                        diamondsCount.text = currentVisibleValue.ToString();
                                    }, currentVisibleValue, 0.5f)
                                    .SetAutoKill(false)
                                    .Pause();

        CheckStartTimer(completedSeconds);
    }

    private void AddCard()
    {
        if (_cardHandler.Value < max)
        {
            _cardHandler.Value++;
        }
    }

    private void ChangeValue(int diamondsCount, int direction)
    {
        _textPunchTween.Restart();
        _counterTweener.ChangeEndValue(diamondsCount, true).Restart();


        if (_cardHandler.Value < max)
        {
            if (_reloadTween == null)
            {
                PlayerPrefs.SetString("lastCardAddTime", DateTime.UtcNow.Ticks.ToString());
                CheckStartTimer();
            }
        }
        else
        {
            _reloadTween?.Pause();
            timerText.gameObject.SetActive(false);
        }
    }

    private void CheckStartTimer(int completedSeconds = 0)
    {
        if (_cardHandler.Value < max)
        {
            if (_reloadTween == null)
            {
                timerText.text = new DateTime().AddSeconds(reloadDuration).ToString("H:mm:ss");
                timerText.gameObject.SetActive(true);

                _reloadTween = DOTween.Sequence().AppendInterval(1)
                                                .SetLoops(reloadDuration - completedSeconds)
                                                .OnStepComplete(() => timerText.text = RemainingText)
                                                .OnComplete(() => { _reloadTween = null; AddCard(); });
            }
        }
        else
        {
            _reloadTween?.Pause();
            timerText.gameObject.SetActive(false);
        }
    }
}
