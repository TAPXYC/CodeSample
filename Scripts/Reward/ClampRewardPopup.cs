using System;
using System.Collections;
using System.Collections.Generic;
using NiobiumStudios;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClampRewardPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tbRewardMessage;
    [SerializeField] TextMeshProUGUI tbRewardCount;                     // Reward Text to show an explanatory message to the player
    [SerializeField] Button buttonCloseReward;            // The Button to close the Rewards Panel
    [SerializeField] Image imageReward;                   // The image of the reward

    private Reward _currentReward;
    private DBController _db;

    public void Init()
    {
        gameObject.SetActive(false);
        buttonCloseReward.onClick.AddListener(Clamp);
        _db = GameManager.Inst.FirebaseController.DataBase;
    }

    private void Clamp()
    {
        if (_currentReward.unit == CurrencyType.Diamond)
        {
            _db.DiamondHandler.Value += _currentReward.reward;
        }
        if (_currentReward.unit == CurrencyType.Cards)
        {
            _db.CardHandler.Value += _currentReward.reward;
        }
        gameObject.SetActive(false);
    }

    public void Show(Reward reward, string message)
    {
        _currentReward = reward;
        gameObject.SetActive(true);

        var rewardQt = reward.reward;
        imageReward.sprite = reward.sprite;
        tbRewardMessage.text = message;

        if (rewardQt > 0)
        {
            tbRewardCount.text = $"+ {rewardQt}";
        }
        else
        {
            tbRewardCount.text = "+ 1";
        }
    }

}
