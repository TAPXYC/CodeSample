using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuDiamondHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI diamondsCount;
    [SerializeField] Button clickButton;

    public event Action OnClick;

    private IntegerHandler _diamondHandler;
    private Tweener _counterTweener;
    private Tween _textPunchTween;

    public void Init()
    {
        _diamondHandler = GameManager.Inst.FirebaseController.DataBase.DiamondHandler;
        _diamondHandler.OnChangeValue += ChangeValue;

        clickButton.onClick.AddListener(() => OnClick?.Invoke());

        int currentVisibleValue = _diamondHandler.Value;

        diamondsCount.text = currentVisibleValue.ToString();

        _textPunchTween = diamondsCount.transform.DOPunchScale(new Vector3(0.4f, 0.2f, 0), 0.3f, 10, 1)
                                                    .SetAutoKill(false)
                                                    .Pause();

        _counterTweener = DOTween.To(() => currentVisibleValue, value => { currentVisibleValue = value; diamondsCount.text = currentVisibleValue.ToString(); }, currentVisibleValue, 0.5f)
                                    .SetAutoKill(false)
                                    .Pause();
    }


    private void ChangeValue(int remainingDiamondsCount)
    {
        _textPunchTween.Restart();
        _counterTweener.ChangeEndValue(remainingDiamondsCount, true).Restart();
    }
}
