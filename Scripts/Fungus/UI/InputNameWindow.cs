using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InputNameWindow : MonoBehaviour
{
    [Tooltip("The continue button UI object")]
    [SerializeField] protected Button continueButton;
    [Tooltip("Input Field")]
    [SerializeField] protected TMP_InputField inputField;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] float fadeTime = 0.5f;

    public string InputedText
    {
        get;
        private set;
    }

    private bool _inputComplete;
    private Tweener _fadeTweener;
    private bool _isInited = false;


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (!_isInited)
        {
            _isInited = true;
            gameObject.SetActive(false);
            continueButton.onClick.AddListener(Next);
            _fadeTweener = canvasGroup.DOFade(1f, fadeTime)
                                        .SetAutoKill(false)
                                        .Pause();
        }
    }

    public IEnumerator Show(string placeholder)
    {
        Init();
        inputField.text = string.Empty;
        (inputField.placeholder as TextMeshProUGUI).text = placeholder;
        gameObject.SetActive(true);
        _inputComplete = false;
        InputedText = string.Empty;

        _fadeTweener.ChangeEndValue(1f, true).Restart();

        yield return new WaitUntil(() => _inputComplete);
        InputedText = inputField.text == string.Empty ? placeholder : inputField.text;

        _fadeTweener.ChangeEndValue(0f, true).Restart();
        yield return _fadeTweener.WaitForCompletion();
        gameObject.SetActive(false);
    }


    private void Next()
    {
        //if (!inputField.text.IsNullOrEmpty())
        _inputComplete = true;
    }
}
