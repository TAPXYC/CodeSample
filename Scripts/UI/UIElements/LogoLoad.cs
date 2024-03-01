using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LogoLoad : MonoBehaviour
{
    [SerializeField] RectTransform baseContent;
    [Space]
    [SerializeField] Image baseFilling;
    [SerializeField] Image fill;
    [SerializeField] TMP_Text loading;


    private IEnumerator LoadCoroutine()
    {
        int loadingProgress = 0;
        loading.text = "0 %";
        baseFilling.fillAmount = 1f;
        fill.fillAmount = 0;

        while (loadingProgress < 100)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.02f, 0.15f));
            loadingProgress = Mathf.Clamp(loadingProgress + UnityEngine.Random.Range(1, 15), 0, 100);

            float progress = loadingProgress / 100f;
            SetFill(progress);

            loading.text = $"{loadingProgress} %";
        }


    }



    private void SetFill(float progress)
    {
        baseFilling.fillAmount = 1f - progress;
        fill.fillAmount = progress;
    }


    public void StartScreen(bool show)
    {
        StopAllCoroutines();
        gameObject.SetActive(show);
        if (show)
        {
            baseFilling.fillAmount = 1;
            fill.fillAmount = 0;
            StartCoroutine(LoadCoroutine());
        }
        else
        {
            StopAllCoroutines();
        }
    }


    public void StartScreen(Func<TMP_Text, Action<float>, IEnumerator> proccess)
    {
        StopAllCoroutines();

        gameObject.SetActive(true);
        baseFilling.fillAmount = 1;
        fill.fillAmount = 0;
        StartCoroutine(proccess(loading, SetFill));
    }
}
