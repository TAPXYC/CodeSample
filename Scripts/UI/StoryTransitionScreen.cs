using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System;

public class StoryTransitionScreen : MonoBehaviour
{
    [SerializeField] RectTransform baseContent;
    [Space]
    [SerializeField] Image background1;
    [SerializeField] Image background2;
    [Space]
    [SerializeField] Image storyName;
    [SerializeField] TextMeshProUGUI chapterName;
    [Space]
    [SerializeField] TextMeshProUGUI loadingProgress;

    private Coroutine _changeCoroutine;
    private Coroutine _loadPercentCoroutine;
    private float _lastBannerHeight;
    private bool _isShow;

    public void Init()
    {
        gameObject.SetActive(false);
        _isShow = false;
    }


    void Update()
    {
        float bannerHeight = GameManager.ADS.BannerHeight;

        if (!_lastBannerHeight.IsEqFloat(bannerHeight))
        {
            _lastBannerHeight = bannerHeight;
            baseContent.SetBottom(_lastBannerHeight);
        }
    }

    public void Show(StoryDataInfo storyData)
    {
        StopCoroutines();

        storyName.sprite = storyData.Header;
        chapterName.text = $"{storyData.CurrentSeriesInfo.ChapterName} Сезон {storyData.CurrentSeason + 1}";
        gameObject.SetActive(true);
        _isShow = true;

        _changeCoroutine = StartCoroutine(ChangeImageCoroutine(storyData.LoadingBackgrounds, storyData.ChangeInterval, storyData.FadeTime));
        SetLoad(DefaultLoadCoroutine);
    }



    public void SetLoad(Func<TMP_Text, IEnumerator> proccess)
    {
        if (_loadPercentCoroutine != null)
        {
            StopCoroutine(_loadPercentCoroutine);
            _loadPercentCoroutine = null;
        }

        _loadPercentCoroutine = StartCoroutine(proccess(loadingProgress));
    }



    public void Hide()
    {
        if (_isShow)
        {
            StopCoroutines();
            gameObject.SetActive(false);
            _isShow = false;
        }
    }


    private void StopCoroutines()
    {
        if (_changeCoroutine != null)
        {
            StopCoroutine(_changeCoroutine);
            _changeCoroutine = null;
        }

        if (_loadPercentCoroutine != null)
        {
            StopCoroutine(_loadPercentCoroutine);
            _loadPercentCoroutine = null;
        }
    }

    private IEnumerator DefaultLoadCoroutine(TMP_Text tb)
    {
        int loading = 0;
        tb.text = "0 %";

        while (loading <= 100)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.02f, 0.2f));
            loading = Mathf.Clamp(loading + UnityEngine.Random.Range(1, 10), 0, 100);
            tb.text = $"{loading} %";
        }
    }

    private IEnumerator ChangeImageCoroutine(Sprite[] backgrounds, float changeTime, float fadeTime)
    {
        int backgroundIndex = 0;
        Image activeImage = background1;

        activeImage.sprite = backgrounds[backgroundIndex];
        activeImage.transform.SetAsLastSibling();
        activeImage.color = Color.white;

        background2.color = new Color(1, 1, 1, 0);

        while (true)
        {
            yield return new WaitForSeconds(changeTime);
            activeImage.DOFade(0, fadeTime);

            backgroundIndex = (backgroundIndex + 1) % backgrounds.Length;

            activeImage = activeImage == background1 ? background2 : background1;
            activeImage.transform.SetAsLastSibling();
            activeImage.sprite = backgrounds[backgroundIndex];
            activeImage.DOFade(1, fadeTime);

            yield return new WaitForSeconds(fadeTime);
        }
    }
}
