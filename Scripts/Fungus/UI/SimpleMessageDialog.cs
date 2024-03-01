using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SimpleMessageDialog : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button clickArea;
    [SerializeField] float fadeTime = 0.5f;

    private Action _onComplete;
    private Tween _showTween;
    private Sequence _waitSequence;
    private bool _isShow;
    private AudioSource _targetAudioSource;

    public void Init()
    {
        clickArea.onClick.AddListener(Click);

        gameObject.SetActive(true);

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        _isShow = false;

        if (_targetAudioSource == null)
        {
            _targetAudioSource = GetComponent<AudioSource>();
            if (_targetAudioSource == null)
            {
                _targetAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }


    public void Show(string message, Action onComplete, AudioClip voiceOverClip)
    {
        _isShow = true;
        canvasGroup.blocksRaycasts = true;
        _onComplete = onComplete;
        messageText.text = message;

        if (_targetAudioSource != null && voiceOverClip != null)
        {
            _targetAudioSource.loop = false;
            _targetAudioSource.clip = voiceOverClip;
            _targetAudioSource.Play();
        }
        FadeAnimation(1);
    }

    public void Show(string message, float time, Action onComplete, AudioClip voiceOverClip)
    {
        _isShow = true;
        canvasGroup.blocksRaycasts = true;
        _onComplete = onComplete;
        messageText.text = message;

        _waitSequence = DOTween.Sequence().Append(FadeAnimation(1))
                                            .AppendInterval(time)
                                            .AppendCallback(Close);
    }


    private void Close()
    {
        if (_isShow)
        {
            _isShow = false;
            canvasGroup.blocksRaycasts = false;
            _waitSequence.SafeKill();

            _onComplete?.Invoke();
            _onComplete = null;

            FadeAnimation(0);
        }
    }

    private void Click()
    {
        Close();
    }


    private Tween FadeAnimation(float endValue)
    {
        if (_showTween != null)
            _showTween.Kill();

        _showTween = canvasGroup.DOFade(endValue, fadeTime);

        return _showTween;
    }
}
