using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class Switch : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image body;
    [SerializeField] Transform switcherRound;
    [SerializeField] Transform rightPosition;
    [SerializeField] Transform leftPosition;
    [SerializeField] float duration;
    [SerializeField] Color activeColorLight;
    [SerializeField] Color activeColorDark;
    [SerializeField] Color inactiveColor;

    public event Action<bool> OnChangeValue;

    private Color _activeColor;
    private bool _isEnabled = true;
    private Tweener _moveTweener;
    private Tweener _colorTweener;


    public void Init(bool isOn)
    {
        CheckCurrentTheme();

        _moveTweener = switcherRound.DOLocalMove(rightPosition.localPosition, duration)
                                    .SetAutoKill(false)
                                    .SetEase(Ease.OutCubic)
                                    .Pause();

        _colorTweener = body.DOColor(_activeColor, duration)
                            .SetAutoKill(false)
                            .Pause();

        switcherRound.localPosition = isOn ? rightPosition.localPosition : leftPosition.localPosition;
        body.color = isOn ? _activeColor : inactiveColor;

        _isEnabled = isOn;
    }

    private void SwitchAnimation()
    {
        if (_isEnabled)
        {
            DisableSwitch();
        }
        else
        {
            EnableSwitch();
        }
        _isEnabled = !_isEnabled;
        OnChangeValue?.Invoke(_isEnabled);
    }

    void EnableSwitch()
    {
        _moveTweener.ChangeEndValue(rightPosition.localPosition, true)
                    .Restart();
        SetActiveColor();
    }

    void DisableSwitch()
    {
        _moveTweener.ChangeEndValue(leftPosition.localPosition, true)
                    .Restart();
        SetInactiveColor();
    }

    void SetActiveColor()
    {
        _colorTweener.ChangeEndValue(_activeColor, true)
                    .Restart();
    }

    void SetInactiveColor()
    {
        _colorTweener.ChangeEndValue(inactiveColor, true)
                    .Restart();
    }


    void CheckCurrentTheme()
    {
        if (PlayerPrefs.GetInt("theme", 0) == (int)ColorTheme.Light)
        {
            _activeColor = activeColorLight;
        }
        else if (PlayerPrefs.GetInt("theme", 0) == (int)ColorTheme.Dark)
        {
            _activeColor = activeColorDark;
        }
    }


    public void ChangeBodyColor()
    {
        CheckCurrentTheme();
        if (_isEnabled)
            SetActiveColor();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeBodyColor();
        SwitchAnimation();
    }
}
