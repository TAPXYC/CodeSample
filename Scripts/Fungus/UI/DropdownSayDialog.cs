using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using DG.Tweening;
using TMPro;
//608 705 11

public class DropdownSayDialog : SayDialog
{
    [SerializeField] GameObject clickArea;
    [SerializeField] RectTransform baseDialog;
    [SerializeField] float minSize;
    [SerializeField] float showTime = 0.5f;
    [SerializeField] float printDelay = 0.3f;
    [SerializeField] TextMeshProUGUI storyInputText;
    [SerializeField] GameObject sayMode;
    [SerializeField] GameObject thinkMode;
    [Space]
    [SerializeField] RectTransform downPoint;
    [SerializeField] Transform diamondCountPosition;

    public RectTransform DownPoint => downPoint;
    public Transform DiamondCountPosition => diamondCountPosition;

    private Tweener _showTweener;
    private bool _isInited = false;
    private const float _sidePerLine = 10f;

    protected override void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (!_isInited)
        {
            base.Awake();
            _isInited = true;
            gameObject.SetActive(false);
            _showTweener = baseDialog.DOSizeDelta(Vector2.zero, showTime)
                        .SetEase(Ease.OutQuad)
                        .SetAutoKill(false)
                        .Pause();
        }
    }




    public void Say(string speaker, string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, AudioClip voiceOverClip, Action onComplete)
    {
        Init();
        gameObject.SetActive(true);
        Say($"{{w={printDelay}}}" + text, clearPrevious, waitForInput, fadeWhenDone, false, false, voiceOverClip, onComplete);
        nameTextAdapter.Text = speaker;
    }


    public override void Say(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, bool stopVoiceover, bool waitForVO, AudioClip voiceOverClip, Action onComplete)
    {
        Init();
        var currentSizeDelta = baseDialog.sizeDelta;
        ActiveSayDialog = this;

        if (clearPrevious)
        {
            currentSizeDelta.y = minSize;
            baseDialog.sizeDelta = currentSizeDelta;

            SetMode(/*text[0] == '~'*/false);

            if (text[0] == '~')
                text = text.Remove(0, 1);
        }

        float deltaSize = CalculateNewSize(storyInputText.GetTextInfo(text));

        currentSizeDelta.y += deltaSize;
        _showTweener.ChangeEndValue(currentSizeDelta, true).Restart();
        SetClickAreaActive(true);

        gameObject.SetActive(true);

        base.Say($"{{w={printDelay}}}" + text, clearPrevious, waitForInput, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, onComplete);
    }




    public void SetClickAreaActive(bool enabled)
    {
        clickArea.SetActive(enabled);
    }


    protected override void OnCompleteText()
    {
        base.OnCompleteText();
    }

    private float CalculateNewSize(TMP_TextInfo textInfo)
    {
        int lineCount = Mathf.Clamp(textInfo.lineCount - 1, 0, 15);
        float heightDelta = (textInfo.lineInfo[0].lineHeight - _sidePerLine) * lineCount;
        //Debug.LogError($"line count - {textInfo.lineCount}, lc - {lineCount}, line height - {textInfo.lineInfo[0].lineHeight}, delta - {heightDelta}");

        //if (lineCount > 0)
        //    heightDelta += 10;

        return heightDelta;
    }

    private void SetMode(bool think)
    {
        if (sayMode != null)
            sayMode.SetActive(!think);
            
        if (thinkMode != null)
            thinkMode.SetActive(think);
    }


}
