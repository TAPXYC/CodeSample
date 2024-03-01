using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    [SerializeField] Transform baseStats;
    [SerializeField] StatHandler statHandlerPrefab;
    [SerializeField] Button closeButton;

    [Space]
    [SerializeField] RectTransform background;


    protected float baseSize = 150;
    protected float anyStatSize = 100;

    private List<StatHandler> _statHandlers = new List<StatHandler>();


    public virtual void Init()
    {
        baseSize = background.sizeDelta.y;
        closeButton.onClick.AddListener(Close);
        gameObject.SetActive(false);
    }

    protected virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        Clear();

        foreach (var stat in GameManager.Story.StoryStage.MainCharacter.Stats.Where(s => s.IsVisible))
        {
            StatHandler statHandler = Instantiate(statHandlerPrefab, baseStats);
            statHandler.Init(stat);
            _statHandlers.Add(statHandler);
        }
        
        if (background != null)
            background.sizeDelta = new Vector2(background.sizeDelta.x, baseSize + anyStatSize * _statHandlers.Count());

        gameObject.SetActive(true);
    }


    protected virtual void Clear()
    {
        foreach (var statHandler in _statHandlers)
        {
            Destroy(statHandler.gameObject);
        }

        if (background != null)
            background.sizeDelta = new Vector2(background.sizeDelta.x, baseSize);

        _statHandlers.Clear();
    }
}
