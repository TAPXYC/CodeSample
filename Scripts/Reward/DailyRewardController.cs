using System;
using NiobiumStudios;
using UnityEngine;

public class DailyRewardController : MonoBehaviour
{
    [SerializeField] DailyRewards dailyRewards;          // DailyReward Instance   
    [Space]
    [SerializeField] DailyDiamondBonus[] dailyRewardList;


    [Serializable]
    public struct DailyDiamondBonus
    {
        public int day;
        public Reward reward;
    }

    public event Action<Reward> OnClaimedBonusReward;
    public event Action OnReadyToBonusClaim;
    public event Action<string> OnTick;
    public event Func<bool> OnForceUpdate;


    public bool HasBonusReward => dailyRewards.availableBonusReward > 0;
    public int LastBonusReward => dailyRewards.lastBonusReward;
    public int AvailableBonusReward => dailyRewards.availableBonusReward;
    public Reward[] Rewards => dailyRewards.rewards.ToArray();

    private bool readyToClaim;                  // Update flag


    public void Init()
    {
        dailyRewards.onClaimDailyPrize += ClaimDailyPrize;
        dailyRewards.OnCompleteInit += CheckDailyBonus;

        dailyRewards.Init();
    }

    private void CheckDailyBonus()
    {
        TimeSpan dailyTimeDifference = dailyRewards.GetDailyTimeDifference();

        if (dailyTimeDifference.TotalSeconds <= 0)
        {
            dailyRewards.ClaimDailyPrize();
        }
    }



    void Update()
    {
        if (dailyRewards.IsLoaded)
        {
            dailyRewards.TickTime();
            CheckTimeDifference();
        }
    }



    public void ClaimBonusReward()
    {
        dailyRewards.ClaimBonusPrize();
        readyToClaim = false;
    }


    // Delegate
    private void OnClaimBonusPrize(int day)
    {
        var reward = dailyRewards.GetBonusReward(day);
        OnClaimedBonusReward?.Invoke(reward);
    }


    private void ClaimDailyPrize(int day)
    {
        Reward reward = dailyRewardList[0].reward;

        for (int i = 0; i < dailyRewardList.Length; i++)
        {
            if (day < dailyRewardList[i].day)
            {
                break;
            }
            else
                reward = dailyRewardList[i].reward;
        }
        GameManager.Inst.UIController.ClampReward.Show(reward, $"Награда за {day} день");
    }



    private void CheckTimeDifference()
    {
        if (!readyToClaim && dailyRewards.IsLoaded)
        {
            TimeSpan bonusTimeDifference = dailyRewards.GetBonusTimeDifference();

            if (bonusTimeDifference.TotalSeconds <= 0)
            {
                readyToClaim = true;
                dailyRewards.CheckBonusRewards();
                dailyRewards.CheckDailyRewards();
                OnReadyToBonusClaim?.Invoke();
            }
            else
            {
                string formattedTs = dailyRewards.GetFormattedTime(bonusTimeDifference);
                OnTick?.Invoke($"{formattedTs}");
            }
        }
    }

    private void OnInitialize(bool error, string errorMessage)
    {
        if (!error)
        {
            if (OnForceUpdate != null)
                readyToClaim = OnForceUpdate.Invoke();

            var showWhenNotAvailable = dailyRewards.keepOpen;
            var isRewardAvailable = dailyRewards.availableBonusReward > 0;
            //canvas.gameObject.SetActive(showWhenNotAvailable || (!showWhenNotAvailable && isRewardAvailable));

            CheckTimeDifference();
        }
        else
        {
            Debug.LogError(errorMessage);
        }
    }



    void OnEnable()
    {
        dailyRewards.onClaimBonusPrize += OnClaimBonusPrize;
        dailyRewards.onInitialize += OnInitialize;
    }

    void OnDisable()
    {
        if (dailyRewards != null)
        {
            dailyRewards.onClaimBonusPrize -= OnClaimBonusPrize;
            dailyRewards.onInitialize -= OnInitialize;
        }
    }



    //#if UNITY_EDITOR
    void OnGUI()
    {
        if (GameManager.ShowDebug)
        {
            int x = 10;
            int delta = 10;

            if (GUI.Button(new Rect(x, 10, 140, 30), "Подождать 1 день"))
            {
                dailyRewards.bonusDebugTime = dailyRewards.bonusDebugTime.Add(new TimeSpan(1, 0, 0, 0));
                dailyRewards.dailyDebugTime = dailyRewards.dailyDebugTime.Add(new TimeSpan(1, 0, 0, 0));
                dailyRewards.SaveDebugTime();
                dailyRewards.CheckBonusRewards();
                dailyRewards.CheckDailyRewards();
                readyToClaim = OnForceUpdate.Invoke();
            }

            x += 140 + delta;

            if (GUI.Button(new Rect(x, 10, 140, 30), "Подождать 1 час"))
            {
                dailyRewards.bonusDebugTime = dailyRewards.bonusDebugTime.Add(new TimeSpan(1, 0, 0));
                dailyRewards.dailyDebugTime = dailyRewards.dailyDebugTime.Add(new TimeSpan(1, 0, 0));
                dailyRewards.SaveDebugTime();
                dailyRewards.CheckBonusRewards();
                dailyRewards.CheckDailyRewards();
                readyToClaim = OnForceUpdate.Invoke();
            }

            x += 140 + delta;

            if (GUI.Button(new Rect(x, 10, 80, 30), "Сбросить"))
            {
                dailyRewards.Reset();
                readyToClaim = false;
            }
        }
    }
    //#endif
}
