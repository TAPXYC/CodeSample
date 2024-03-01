using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EndScreen : StatsScreen
{
    [Space]
    [SerializeField] int diamondsForComplete = 3;
    [SerializeField] Transform baseDiamondsInfo;
    [SerializeField] TextMeshProUGUI diamondsCount;
    [Space]
    [SerializeField] Button adsButton;
    [SerializeField] int diamondsForReward = 3;
    [SerializeField] Image backgroundImage;

    public event Action OnComplete;

    private IntegerHandler _diamondHandler;

    public override void Init()
    {
        base.Init();
        adsButton.gameObject.SetActive(true);
    }

    public void SetBackground(Sprite background)
    {
        backgroundImage.sprite = background;
    }

    private void ShowReward()
    {
        //показ рекламы 
        GameManager.ADS.TryShowReward(success =>
        {
            if (success)
            {
                _diamondHandler.Value += diamondsForReward;
                diamondsCount.text = $"{_diamondHandler.Value} (+{diamondsForComplete + diamondsForReward})";

                adsButton.onClick.RemoveAllListeners();
                adsButton.gameObject.SetActive(false);
            }
        });

    }

    public override void Show()
    {
        _diamondHandler = GameManager.Inst.FirebaseController.DataBase.DiamondHandler;
        _diamondHandler.Value += diamondsForComplete;

        adsButton.gameObject.SetActive(true);
        adsButton.onClick.AddListener(ShowReward);

        base.Show();
        baseDiamondsInfo.SetAsLastSibling();

        diamondsCount.text = $"{_diamondHandler.Value} (+{diamondsForComplete})";
    }


    protected override void Clear()
    {
        base.Clear();
    }

    protected override void Close()
    {
        OnComplete?.Invoke();
    }
}
