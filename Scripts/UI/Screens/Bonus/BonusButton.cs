using System;
using System.Collections;
using System.Collections.Generic;
using NiobiumStudios;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    [SerializeField] Button clickArea;

    [Header("Полученные бонусы")]
    [SerializeField] GameObject baseClaimed;
    //[SerializeField] TMP_Text claimedRewardAmount;
    //[SerializeField] Image claimedIcon;

    [Header("Готовые к получению")]
    [SerializeField] GameObject baseAwailable;
    [SerializeField] TMP_Text awailableDayText;
    [SerializeField] TMP_Text awailableRewardAmount;
    [SerializeField] Image awailableIcon;

    [Header("Не готовые к получению")]
    [SerializeField] GameObject baseUnawailable;
    [SerializeField] TMP_Text dayText;
    [SerializeField] TMP_Text rewardAmount;
    [SerializeField] Image icon;

    public int Day
    {
        get;
        private set;
    }

    public Reward Reward
    {
        get;
        private set;
    }


    public event Action OnClaim;


    // The States a reward can have
    public enum DailyRewardState
    {
        UNCLAIMED_AVAILABLE,
        UNCLAIMED_UNAVAILABLE,
        CLAIMED
    }


    private DailyRewardState state;


    public void Initialize(int day, Reward reward)
    {
        Day = day;
        Reward = reward;

        awailableDayText.text = dayText.text = $"День {day}";
        awailableRewardAmount.text = rewardAmount.text = reward.reward.ToString();
        awailableIcon.sprite = icon.sprite = reward.sprite;
        awailableIcon.color = icon.color = reward.color;
        clickArea.onClick.AddListener(Claim);

        Refresh();
    }

    private void Claim()
    {
        OnClaim?.Invoke();
    }



    // Refreshes the UI
    public bool CheckState(int availableReward, int lastReward)
    {
        bool isRewardAvailableNow = false;

        if (Day == availableReward)
        {
            state = DailyRewardState.UNCLAIMED_AVAILABLE;

            isRewardAvailableNow = true;
        }
        else if (Day <= lastReward)
        {
            state = DailyRewardState.CLAIMED;
        }
        else
        {
            state = DailyRewardState.UNCLAIMED_UNAVAILABLE;
        }

        return isRewardAvailableNow;
    }


    public void Refresh()
    {
        switch (state)
        {
            case DailyRewardState.UNCLAIMED_AVAILABLE:
                baseClaimed.SetActive(false);
                baseAwailable.SetActive(true);
                baseUnawailable.SetActive(false);
                clickArea.interactable = true;
                break;
            case DailyRewardState.UNCLAIMED_UNAVAILABLE:
                baseClaimed.SetActive(false);
                baseAwailable.SetActive(false);
                baseUnawailable.SetActive(true);
                clickArea.interactable = false;
                break;
            case DailyRewardState.CLAIMED:
                baseClaimed.SetActive(true);
                baseAwailable.SetActive(false);
                baseUnawailable.SetActive(false);
                clickArea.interactable = false;
                break;
        }
    }
}
