using System.Collections.Generic;
using NiobiumStudios;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusesMenu : MonoBehaviour
{
    [SerializeField] BonusButton dailyRewardButtonPrefab;        // Prefab containing each daily reward
    [Header("window")]
    [SerializeField] GameObject baseScreen;
    [SerializeField] Button buttonClose;                  // Close Button
    [SerializeField] TextMeshProUGUI textTimeDue;                    // Text showing how long until the next claim
    [SerializeField] GridLayoutGroup dailyRewardsGroup;   // The Grid that contains the rewards
    [SerializeField] ScrollRect scrollRect;               // The Scroll Rect
    //[Space]
    //[SerializeField] ClampRewardPopup clampReward;
    [Space] 
    [SerializeField] Animation anim;
	


    private List<BonusButton> dailyRewardButtons = new List<BonusButton>();
    private DailyRewardController _dailyRewardController;


    public void Init(DailyRewardController dailyRewardController)
    {
        _dailyRewardController = dailyRewardController;
        _dailyRewardController.OnClaimedBonusReward += OnClaimPrize;
        _dailyRewardController.OnReadyToBonusClaim += SetReadyToClaim;
        _dailyRewardController.OnTick += UpdateTimerValue;
        _dailyRewardController.OnForceUpdate += UpdateUI;

        InitializeDailyRewardsUI(_dailyRewardController.Rewards);

        buttonClose.onClick.AddListener(() => Show(false));

        UpdateUI();

        gameObject.SetActive(true);
        baseScreen.SetActive(false);
        //clampReward.Init();

        //if (_dailyRewardController.HasBonusReward)
        //    ClaimReward();
    }



    public void Show(bool show)
    {
        if (show)
        {
            UpdateUI();
        }

        baseScreen.SetActive(show);
        anim.Play();
    }



    private void ClaimReward()
    {
        _dailyRewardController.ClaimBonusReward();
        UpdateUI();
    }


    // Delegate
    private void OnClaimPrize(Reward reward)
    {
        var _db = GameManager.Inst.FirebaseController.DataBase;

        if (reward.unit == CurrencyType.Diamond)
        {
            _db.DiamondHandler.Value += reward.reward;
    }
        if (reward.unit == CurrencyType.Cards)
        {
            _db.CardHandler.Value += reward.reward;
        }
        //clampReward.Show(reward, $"Бонус за {day} день");
    }



    // Initializes the UI List based on the rewards size
    private void InitializeDailyRewardsUI(Reward[] rewards)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            var reward = rewards[i];
            int day = i + 1;

            BonusButton dailyReward = Instantiate(dailyRewardButtonPrefab);

            dailyReward.transform.SetParent(dailyRewardsGroup.transform);
            dailyReward.transform.localScale = Vector2.one;
            dailyReward.Initialize(day, reward);
            dailyReward.OnClaim += ClaimReward;

            dailyRewardButtons.Add(dailyReward);
        }
    }

    public bool UpdateUI()
    {
        bool isRewardAvailableNow = false;

        int availableReward = _dailyRewardController.AvailableBonusReward;
        int lastReward = _dailyRewardController.LastBonusReward;

        foreach (var dailyRewardButton in dailyRewardButtons)
        {
            isRewardAvailableNow |= dailyRewardButton.CheckState(availableReward, lastReward);
            dailyRewardButton.Refresh();
        }

        if (isRewardAvailableNow)
        {
            SnapToReward();
            textTimeDue.text = "Можете забрать награду!";
        }

        return isRewardAvailableNow;
    }


    // Snap to the next reward
    public void SnapToReward()
    {
        Canvas.ForceUpdateCanvases();

        var lastRewardIdx = _dailyRewardController.LastBonusReward;

        // Scrolls to the last reward element
        if (dailyRewardButtons.Count - 1 < lastRewardIdx)
            lastRewardIdx++;

        if (lastRewardIdx > dailyRewardButtons.Count - 1)
            lastRewardIdx = dailyRewardButtons.Count - 1;

        var target = dailyRewardButtons[lastRewardIdx].GetComponent<RectTransform>();
        var content = scrollRect.content;

        //content.anchoredPosition = (Vector2)scrollRect.transform.InverseTransformPoint(content.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        float normalizePosition = (float)target.GetSiblingIndex() / (float)content.transform.childCount;
        scrollRect.verticalNormalizedPosition = normalizePosition;
    }


    private void SetReadyToClaim()
    {
        UpdateUI();
        SnapToReward();
        textTimeDue.text = "Можете забрать награду!";
    }

    private void UpdateTimerValue(string formattedTs)
    {
        textTimeDue.text = $"Следующая награда через {formattedTs}";
    }
}
