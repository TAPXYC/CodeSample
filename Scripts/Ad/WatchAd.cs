using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatchAd : MonoBehaviour
{    
    [SerializeField] Button closeButton;
    [SerializeField] Button watchButton;

    private Action<bool> _onClick;

    public void ShowWindow(Action<bool> onClick)
    {
        _onClick = onClick;
        closeButton.onClick.AddListener(CloseWindow);
        watchButton.onClick.AddListener(WatchClick);
        gameObject.SetActive(true);
    }

    private void CloseWindow()
    {
        _onClick?.Invoke(false);
        Close();
    }

    private void WatchClick()
    {
        _onClick?.Invoke(true);
        Close();
    }


    private void Close()
    {
        gameObject.SetActive(false);
        closeButton.onClick.RemoveAllListeners();
        watchButton.onClick.RemoveAllListeners();
    }
}
