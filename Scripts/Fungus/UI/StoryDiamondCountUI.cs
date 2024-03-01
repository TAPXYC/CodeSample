using UnityEngine;
using DG.Tweening;
using TMPro;

public class StoryDiamondCountUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI diamondsCount;
    [SerializeField] CanvasGroup canvasGroup;

    private IntegerHandler _diamondHandler;
    private Tweener _counterTweener;
    private Tweener _showTween;
    private Tween _textPunchTween;
    private int _currentVisibleValue;
    private bool _isShow;
    private Transform _startTransform;

    public void Init()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        _isShow = false;

        _diamondHandler = GameManager.Inst.FirebaseController.DataBase.DiamondHandler;
        _diamondHandler.OnChangeValue += ChangeValue;

        _currentVisibleValue = _diamondHandler.Value;

        diamondsCount.text = _currentVisibleValue.ToString();

        _textPunchTween = diamondsCount.transform.DOPunchScale(new Vector3(0.4f, 0.2f, 0), 0.3f, 10, 1)
                                                    .SetAutoKill(false)
                                                    .Pause();

        _showTween = canvasGroup.DOFade(1, 0.5f).SetAutoKill(false)
                                                .Pause();

        _counterTweener = DOTween.To(() => _currentVisibleValue, value => { _currentVisibleValue = value; diamondsCount.text = _currentVisibleValue.ToString(); }, _currentVisibleValue, 0.5f)
                                    .SetAutoKill(false)
                                    .Pause();
    }


    public void Show(Transform parentPosition)
    {
        if (transform.parent != parentPosition)
        {
            _isShow = false;
        }

        if (!_isShow)
        {
            canvasGroup.alpha = 0;
            transform.SetParent(parentPosition);
            transform.localPosition = Vector3.zero;

            SetShow(true);
        }
    }

    public void Hide()
    {
        SetShow(false);
    }


    public void Stash()
    {
        transform.SetParent(_startTransform);
    }

    public int GetDiamondCount()
    {
        return _diamondHandler.Value;
    }

    public void ChangeDiamond(int deltaValue)
    {
        _diamondHandler.Value += deltaValue;
    }

    private void ChangeValue(int remainingDiamondsCount)
    {
        _textPunchTween.Restart();
        _counterTweener.ChangeEndValue(remainingDiamondsCount, true).Restart();
    }

    private void SetShow(bool show)
    {
        if (_isShow != show)
        {
            _isShow = show;
            _showTween.ChangeEndValue(_isShow ? 1f : 0f, true).Restart(true);
        }
    }
}
