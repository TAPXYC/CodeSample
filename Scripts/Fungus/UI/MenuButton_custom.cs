using UnityEngine;
using TMPro;
using DG.Tweening;

public class MenuButton_custom : MonoBehaviour
{
    [SerializeField] RectTransform textTransform;
    [SerializeField] RectTransform idleTextPosition;
    [SerializeField] RectTransform diamondTextPosition;
    [Space]
    [SerializeField] GameObject cristalModeGO;
    [SerializeField] TextMeshProUGUI cristallsCountText;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeTime = 0.7f;
    [SerializeField] GameObject noManeyBlockGO;

public string ID
{
    get;
    private set;
}

    public int DiamondsCount
    {
        get;
        private set;
    }
    private Tween _fadeTween;

    void Awake()
    {
        _fadeTween = canvasGroup.DOFade(1, fadeTime)
                                  .From(0)
                                  .SetAutoKill(false)
                                  .Pause();
    }


    void OnEnable()
    {
        _fadeTween.Restart();
    }


    public void SetCristalMode(int requireDiamonds, int currentDiamond, string id)
    {
        ID = id;
        SetRectTransform(textTransform, diamondTextPosition);
        DiamondsCount = requireDiamonds;
        noManeyBlockGO.SetActive(requireDiamonds > currentDiamond);
        cristalModeGO.SetActive(true);
        cristallsCountText.text = $"{DiamondsCount}";
    }

    public void SetSimpleMode()
    {
        SetRectTransform(textTransform, idleTextPosition);
        DiamondsCount = -1;
        cristalModeGO.SetActive(false);
        noManeyBlockGO.SetActive(false);
    }


    /// <summary>
    /// Performs a deep copy of all values from one RectTransform to another.
    /// </summary>
    private void SetRectTransform(RectTransform target, RectTransform from)
    {
        //target.eulerAngles = from.eulerAngles;
        target.position = from.position;
        //target.rotation = from.rotation;
        target.anchoredPosition = from.anchoredPosition;
        target.sizeDelta = from.sizeDelta;
        target.anchorMax = from.anchorMax;
        target.anchorMin = from.anchorMin;
        target.pivot = from.pivot;
        //target.localScale = from.localScale;
    }
}
