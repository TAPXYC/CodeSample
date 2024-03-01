using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class StoryMenuController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Transform baseMenu;
    [SerializeField] Transform showPosition;
    [SerializeField] Transform hidePosition;
    [Space]
    [SerializeField] float hideDelay = 2;
    [Space]
    [SerializeField] Button backButton;
    [SerializeField] Button statsButton;
    [SerializeField] Button itemsButton;
    [SerializeField] ThemeButton themeButton;


    public event Action OnMainMenuClick;
    public event Action OnShowStatsClick;
    public event Action OnShowItemsChangeScreenClick;


    private Tweener _showTweener;
    private Sequence _delaySequence;

    public void Init()
    {
        _showTweener = baseMenu.DOMove(showPosition.position, 0.3f)
                                .SetAutoKill(false)
                                .Pause();

        _delaySequence = DOTween.Sequence().AppendInterval(hideDelay)
                                            .AppendCallback(() => ShowMenu(false))
                                            .SetAutoKill(false)
                                            .Pause();

        baseMenu.localPosition = hidePosition.localPosition;

        backButton.onClick.AddListener(MainMenuClick);
        statsButton.onClick.AddListener(ShowStatsClick);
        itemsButton.onClick.AddListener(ShowItemsChangeScreenClick);
        themeButton.Init();
    }

    private void ShowStatsClick()
    {
        OnShowStatsClick?.Invoke();
    }

    private void ShowItemsChangeScreenClick()
    {
        OnShowItemsChangeScreenClick?.Invoke();
    }

    private void MainMenuClick()
    {
        OnMainMenuClick?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowMenu(true);
        _delaySequence.Restart();
    }


    private void ShowMenu(bool show)
    {
        _showTweener.ChangeEndValue(show ? showPosition.position : hidePosition.position, true).Restart();
    }


    void OnDestroy()
    {
        _showTweener.Kill();
        _delaySequence.Kill();
    }
}
