using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System;
using System.Collections;
using System.Linq;

public class StoryController : MonoBehaviour
{
    [SerializeField] StoryStage storyStage;
    [SerializeField] Flowchart flowchart;
    [SerializeField] StoryUIController uIController;

    public event Action<bool> OnCompleteStory;

    public StoryStage StoryStage => storyStage;
    public StoryUIController UIController => uIController;
    public ItemCollection Items => _items;

    public static bool IsLoaded
    {
        get;
        private set;
    } = false;

    public bool IsLoadedFromData
    {
        get;
        private set;
    } = false;

    private SaveManager _saveManager;
    private ItemCollection _items;
    private StoryDataHandler _storyDataHandler;
    private bool _loadedLastPoint;
    private string _saveKey;


    public void Init(StoryDataHandler storyDataHandler, ItemCollection items, Action onComplete, Sprite endScreenBacground)
    {
        SayDialog.ClearSayDialogs();

        _items = items;
        IsLoaded = false;
        IsLoadedFromData = false;

        _storyDataHandler = storyDataHandler;
        _saveKey = _storyDataHandler.FungusSaveKey;
        _storyDataHandler.SetAwailableItems(_items.GetItemsByType(SelectableItemType.Skin).Select(s => s.GetID()).ToArray());
        _saveManager = FungusManager.Instance.SaveManager;

        var selectedItems = _items.GetItemsByID(_storyDataHandler.SelectedItemsID);

        if (selectedItems.IsNullOrEmpty())
        {
            selectedItems = new ISelectableInfo[]
            {
                _items.GetItemsByType(SelectableItemType.Skin)[0],
                _items.GetItemsByType(SelectableItemType.Dress)[0],
                _items.GetItemsByType(SelectableItemType.Makeup)[0],
                _items.GetItemsByType(SelectableItemType.Hair)[0],
            };

            _storyDataHandler.SetSelectedItems(selectedItems.Select(s => s.GetID()).ToArray());
        }

        storyStage.Init(selectedItems);
        uIController.Init(endScreenBacground);
        uIController.OnMainMenu += ExitStory;

        uIController.EndScreen.OnComplete += CompleteStory;
        _saveManager.Load(_saveKey, StoryLoaded);

        StartCoroutine(LoadCoroutine(onComplete));
    }


    public void SetItems(ISelectableInfo[] items)
    {
        _storyDataHandler.SetSelectedItems(items.Select(i => i.GetID()).ToArray());
    }


    public bool HasAwailableItem(ISelectableInfo item)
    {
        return HasAwailableItem(item.GetID());
    }


    public bool HasAwailableItem(string itemID)
    {
        return _storyDataHandler.GetCurrentAwailableItems().Any(ai => ai == itemID);
    }



    public bool HasMenuID(string menuID)
    {
        return _storyDataHandler.SelectedMenus.Contains(menuID);
    }


    public void AddMenuID(string menuID)
    {
        _storyDataHandler.SelectedMenus.Add(menuID);
        _storyDataHandler.SaveSelectedMenus();
    }


    public void SetAwailableItems(ISelectableInfo[] items)
    {
        SetAwailableItems(items.Select(i => i.GetID()).ToArray());
    }

    public void SetAwailableItems(string[] items)
    {
        _storyDataHandler.SetAwailableItems(items);
    }



    public ISelectableInfo[] GetAwailableItems()
    {
        return _items.GetItemsByID(_storyDataHandler.GetCurrentAwailableItems())
                                            .OrderByDescending(g => g.GetItemType())
                                            .ToArray();
    }




    private void StoryLoaded(bool loadedLastPoint)
    {
        _loadedLastPoint = loadedLastPoint;

        if (_loadedLastPoint)
        {
            //Debug.LogError("Story Loaded YES!");
        }
        else
        {
            //Debug.LogError("No");
        }
    }

    private IEnumerator LoadCoroutine(Action onComplete)
    {
        yield return new WaitUntil(() => _loadedLastPoint);

        if (_storyDataHandler.Series > 0 && _storyDataHandler.FirstLoad)
        {
            bool loadedVariables = false;
            StoryData[] storyDatas = new StoryData[0];

            _storyDataHandler.LoadSeriesVariables(_storyDataHandler.Series - 1, datas =>
            {
                storyDatas = datas;
                loadedVariables = true;
            });

            yield return new WaitUntil(() => loadedVariables);

            foreach (var data in storyDatas)
            {
                SetVariable(data);
            }
        }
        IsLoaded = true;
        onComplete?.Invoke();
    }


    private void CompleteStory()
    {
        _storyDataHandler.SaveSeriesVariables(GetSeriesVariables());
        _storyDataHandler.SaveSeriesSelectedItems(storyStage.MainCharacter.GetAllSelectedItems().Select(i => i.GetID()).ToArray());
        OnCompleteStory?.Invoke(true);
        Stash();
    }

    private void ExitStory()
    {
        OnCompleteStory?.Invoke(false);
        Stash();
    }

    public StoryData[] GetSeriesVariables()
    {
        var mainCharacter = storyStage.MainCharacter;
        return mainCharacter.Stats.Where(d => d.SendToNextStory)
                                    .Select(d => d.GetStoryData())
                                    .Append(new StoryData(mainCharacter.CharacterName.stringRef.Key, mainCharacter.CharacterName.Value, DataType.String))
                                    .ToArray();
    }


    private void SetVariable(StoryData data)
    {
        switch (data.DataType)
        {
            case DataType.Int:
                flowchart.SetIntegerVariable(data.Name, int.Parse(data.Value));
                break;
            case DataType.Float:
                flowchart.SetFloatVariable(data.Name, float.Parse(data.Value));
                break;
            case DataType.String:
                flowchart.SetStringVariable(data.Name, data.Value);
                break;
            case DataType.Bool:
                flowchart.SetBooleanVariable(data.Name, bool.Parse(data.Value));
                break;
        }
    }



    public void Save()
    {
        _saveManager.Save(_saveKey);
    }

    private void Stash()
    {
        Save();
        _storyDataHandler.SaveSeriesDataToServer();
        SayDialog.ClearSayDialogs();
        MenuDialog.ActiveMenuDialog = null;
        OnCompleteStory = null;
    }



    protected virtual void OnEnable()
    {
        SaveManagerSignals.OnSavePointAdded += OnSavePointAdded;
        SaveManagerSignals.OnSavePointLoaded += OnSavePointLoaded;
    }


    protected virtual void OnDisable()
    {
        SaveManagerSignals.OnSavePointAdded -= OnSavePointAdded;
        SaveManagerSignals.OnSavePointLoaded -= OnSavePointLoaded;
    }

    protected virtual void OnSavePointLoaded(string savePointKey)
    {
        IsLoadedFromData = true;
        StoryLoaded(true);
    }




    protected virtual void OnSavePointAdded(string savePointKey, string savePointDescription)
    {
        if (_saveManager.NumSavePoints > 0)
        {
            _saveManager.Save(_storyDataHandler.FungusSaveKey);
        }
    }



    void OnGUI()
    {
        if (GameManager.ShowDebug)
        {
            if (GUI.Button(new Rect(Camera.main.pixelWidth - 170, 60, 160, 40), "Пропустить серию"))
            {
                CompleteStory();
            }
        }
    }
}
