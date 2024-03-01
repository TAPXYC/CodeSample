using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AddRewardPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI diamondsCount;
    [SerializeField] Button closeButton;

    private Action _onClose;


    public void Show(int rewardCount, Action onClose)
    {
        diamondsCount.text = $"+{rewardCount}";
        closeButton.onClick.AddListener(Close);
        _onClose = onClose;
        gameObject.SetActive(true);
    }

    private void Close()
    {
        closeButton.onClick.RemoveAllListeners();
        _onClose?.Invoke();
        gameObject.SetActive(false);
    }
}
