using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using DG.Tweening;


#if FBON
using Firebase.Database;
using Firebase.Extensions;
#endif


public enum DataType
{
    Int,
    Float,
    String,
    Bool
}

[Serializable]
public struct StoryData
{
    public string Name;
    public string Value;
    public DataType DataType;

    public StoryData(string name, string value, DataType dataType)
    {
        Name = name;
        Value = value;
        DataType = dataType;
    }


    public static DataType GetDataType(Type varType)
    {
        return varType == typeof(IntegerVariable) ? DataType.Int :
                        varType == typeof(StringVariable) ? DataType.String :
                        varType == typeof(FloatVariable) ? DataType.Float :
                                                            DataType.Bool;
    }


    public string Serialize() => $"{Name}:{Value}:{(int)DataType}";

    public static StoryData Deserialize(string data)
    {
        string[] datas = data.Split(':');
        return new StoryData(datas[0], datas[1], (DataType)int.Parse(datas[2]));
    }

    public override string ToString()
    {
        return $"{Name}: {Value} ({DataType})";
    }
}





public class StoryDataHandler
{
    public event Action<int> OnChangeSeries;

    public readonly string StoryID;
    public string FungusSaveKey => GetFungusSaveKey(_series);
    public string GetFungusSaveKey(int series) => (_isLogginDelegate() ? "rem" : "loc") + GetSeriesSaveKey(series);
    public string GetSeriesSaveKey(int series) => $"{StoryID}_{series}";

    public bool IsLoaded => IsLoadedSeriesNum && IsLoadedSeriesUnlocked && IsLoadedSeriesData && IsLoadedSelectedItems && IsLoadedSeriesAwailableItems && IsLoadedSeriesSelectedMenus;

    public bool FirstLoad => _currentStoryData.IsNullOrEmpty();

    public bool IsLoadedSeriesNum
    {
        get;
        private set;
    }

    public bool IsLoadedSeriesUnlocked
    {
        get;
        private set;
    }

    public bool IsLoadedSeriesData
    {
        get;
        private set;
    }

    public bool IsLoadedSeriesSelectedMenus
    {
        get;
        private set;
    }

    public bool IsLoadedSeriesVariables
    {
        get;
        private set;
    }

    public bool IsLoadedSelectedItems
    {
        get;
        private set;
    }

    public bool IsLoadedSeriesAwailableItems
    {
        get;
        private set;
    }

    public string[] SelectedItemsID
    {
        get;
        private set;
    }

    public List<string> SelectedMenus
    {
        get;
        private set;
    }

    public Dictionary<int, List<string>> AwailableStoryItemsID
    {
        get;
        private set;
    }

    public int Series
    {
        get
        {
            return _series;
        }
        set
        {
            if (_series != value)
            {
                _hasChanges = true;
                _series = value;
                _currentStoryData = String.Empty;
                SaveSeriesNum();
                OnChangeSeries?.Invoke(_series);
            }
        }
    }

    public bool SeriesUnlocked
    {
        get
        {
            return _seriesUnlocked;
        }
        set
        {
            if (_seriesUnlocked != value)
            {
                _hasChanges = true;
                _seriesUnlocked = value;
                SaveSeriesUnlocked();
            }
        }
    }

    public string CurrentStoryData
    {
        get
        {
            return _currentStoryData;
        }
        set
        {
            if (value != _currentStoryData)
            {
                _hasChanges = true;
                _currentStoryData = value;
            }
        }
    }


    private bool _hasChanges;
    private string _currentStoryData;
    private int _series;
    private bool _seriesUnlocked;
    private Func<bool> _isLogginDelegate;
    private Func<string, DatabaseReference> _getStoryDataReference;
    private Func<string, int, DatabaseReference> _getSeriesDataReference;

    #region For debug
    private readonly string _remote;
    private readonly string _local;
    private readonly string _save;
    private readonly string _load;
    #endregion


    public StoryDataHandler(string storyID, Func<bool> isLogginDelegate, Func<string, DatabaseReference> getStoryDataReference, Func<string, int, DatabaseReference> getSeriesDataReference)
    {
        StoryID = storyID;
        _remote = DebugX.ColorPart("REMOTE", Color.green);
        _local = DebugX.ColorPart("LOCAL", Color.grey);
        _save = DebugX.ColorPart("SAVE", Color.yellow);
        _load = DebugX.ColorPart("LOAD", new Color(1, 0.7f, 0));
        SelectedItemsID = new string[0];
        SelectedMenus = new List<string>();
        AwailableStoryItemsID = new Dictionary<int, List<string>>();

        _isLogginDelegate = isLogginDelegate;
        _getStoryDataReference = getStoryDataReference;
        _getSeriesDataReference = getSeriesDataReference;

        IsLoadedSeriesNum = false;
        IsLoadedSeriesUnlocked = false;
        IsLoadedSeriesData = false;
        IsLoadedSeriesAwailableItems = false;
        IsLoadedSelectedItems = false;
        IsLoadedSeriesSelectedMenus = false;

        OnChangeSeries += ReloadStoryData;
    }

    public string[] GetCurrentAwailableItems()
    {
        return AwailableStoryItemsID.Where(kvp => kvp.Key <= _series)
                                    .Select(kvp => kvp.Value)
                                    .SelectMany(l => l)
                                    .ToArray();
    }

    private void ReloadStoryData(int currentSeries)
    {
        LoadSeriesData();
        LoadSelectedMenus();
    }


    public void SetSelectedItems(string[] selectedItemsId)
    {
        bool hasChanges = false;

        if (selectedItemsId.Length == SelectedItemsID.Length)
        {
            hasChanges = !selectedItemsId.Select(si => SelectedItemsID.Contains(si)).All(b => b);
        }
        else
        {
            hasChanges = true;
        }

        if (hasChanges)
        {
            DebugX.ColorMessage($"Изменены выбранные предметы - {string.Join(", ", selectedItemsId.Where(si => !SelectedItemsID.Contains(si)))} ({string.Join(", ", SelectedItemsID)})", Color.cyan);
            SelectedItemsID = selectedItemsId;
            _hasChanges = true;
            SaveSelectedItems();

            SetAwailableItems(SelectedItemsID);
        }
        else
        {
            DebugX.ColorMessage($"Не вижу изменений во внешноти  [{string.Join(" | ", SelectedItemsID)}]    ({string.Join(" | ", selectedItemsId)})", Color.cyan);
        }
    }



    public void SetAwailableItems(string[] awailableItemsIds)
    {
        var allAwailableItems = AwailableStoryItemsID.Select(kvp => kvp.Value).SelectMany(l => l);

        List<string> notContainedItems = awailableItemsIds.Where(si => !allAwailableItems.Contains(si)).ToList();

        if (!notContainedItems.IsNullOrEmpty())
        {
            DebugX.ColorMessage("Добавлены новые предметы НА СЕРИЮ - " + string.Join(", ", notContainedItems), Color.cyan);

            if (!AwailableStoryItemsID.ContainsKey(_series))
            {
                AwailableStoryItemsID.Add(_series, notContainedItems);
            }
            else
            {
                AwailableStoryItemsID[_series].AddRange(notContainedItems);
            }

            _hasChanges = true;
            SaveAwailableItems();
        }
        else
        {
            DebugX.ColorMessage($"Ничего нового не добавилось НА СЕРИЮ", Color.cyan);
        }
    }


    public void LoadAll()
    {
        IsLoadedSeriesNum = false;
        IsLoadedSeriesUnlocked = false;
        IsLoadedSeriesData = false;
        IsLoadedSelectedItems = false;
        IsLoadedSeriesAwailableItems = false;
        IsLoadedSeriesSelectedMenus = false;

        LoadSeriesNum(StoryID, loadedValue =>
        {
            if (_series != loadedValue)
            {
                _series = loadedValue;
                OnChangeSeries?.Invoke(_series);
            }
            else
            {
                LoadSeriesData();
                LoadSelectedMenus();
            }

            IsLoadedSeriesNum = true;
        });

        LoadSeriesUnlocked();
        LoadAwailableItems();
        LoadSelectedItems();

        _hasChanges = false;
    }


    #region Series Number
    public void SaveSeriesNum()
    {
        SaveSeriesNum(StoryID, _series);
    }

    public void LoadSeriesNum()
    {
        IsLoadedSeriesNum = false;
        LoadSeriesNum(StoryID, SeriesNumLoaded);
    }

    private void SeriesNumLoaded(int loadedValue)
    {
        if (_series != loadedValue)
        {
            _series = loadedValue;
            OnChangeSeries?.Invoke(_series);
        }

        IsLoadedSeriesNum = true;
        //DebugX.ColorMessage($"SeriesNum - {loadedValue}", Color.magenta);
    }
    #endregion



    #region Series Unlocked
    public void SaveSeriesUnlocked()
    {
        SaveSeriesUnlocked(StoryID, _seriesUnlocked);
    }

    public void LoadSeriesUnlocked()
    {
        IsLoadedSeriesUnlocked = false;
        LoadSeriesUnlocked(StoryID, SeriesUnlockedLoaded);
    }

    private void SeriesUnlockedLoaded(bool loadedValue)
    {
        _seriesUnlocked = loadedValue;
        IsLoadedSeriesUnlocked = true;
        //DebugX.ColorMessage($"SeriesUnlocked - {loadedValue}", Color.magenta);
    }
    #endregion




    #region Series Data
    public void SaveSeriesDataToServer()
    {
        // проверка на изменение данных
        SaveStoryDataToServer(StoryID, _series, SaveManager.GetDataSaveHistory(FungusSaveKey));
    }

    public void LoadSeriesData()
    {
        IsLoadedSeriesData = false;
        LoadStoryData(StoryID, _series, SeriesDataLoaded);
    }


    private void SeriesDataLoaded(string storyDataJSON, bool isLocal)
    {
        CurrentStoryData = storyDataJSON;

        if (!isLocal && !storyDataJSON.IsNullOrEmpty())
            SaveManager.WriteSaveHistory(FungusSaveKey, storyDataJSON);

        IsLoadedSeriesData = true;
    }
    #endregion


    #region Series Variables

    public void SaveSeriesVariables(StoryData[] seriesVariables)
    {
        SaveStoryVariable(StoryID, _series, seriesVariables);
    }

    public void LoadSeriesVariables(int series, Action<StoryData[]> loadedCallback)
    {
        IsLoadedSeriesVariables = false;
        LoadStoryVariable(StoryID, series, seriesVariables => SeriesVariablesLoaded(seriesVariables, loadedCallback));
    }


    private void SeriesVariablesLoaded(StoryData[] seriesVariables, Action<StoryData[]> loadedCallback)
    {
        loadedCallback?.Invoke(seriesVariables);
        IsLoadedSeriesVariables = true;
    }


    #endregion


    #region Awailable Items
    public void SaveAwailableItems()
    {
        SaveAwailableItems(StoryID, String.Join("\t", AwailableStoryItemsID.Select(kvp => $"{kvp.Key}:{String.Join(",", kvp.Value)}")));
    }

    public void LoadAwailableItems()
    {
        IsLoadedSeriesAwailableItems = false;
        LoadAwailableItems(StoryID, AwailableItemsLoaded);
    }

    private void AwailableItemsLoaded(string awailableItems)
    {
        AwailableStoryItemsID.Clear();

        if (!awailableItems.IsNullOrEmpty())
        {
            var entries = awailableItems.Split('\t', StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var kvp = entry.Split(':', StringSplitOptions.RemoveEmptyEntries);

                int storyNum = int.Parse(kvp[0]);
                List<string> items = kvp[1].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

                if (!AwailableStoryItemsID.ContainsKey(storyNum))
                {
                    AwailableStoryItemsID.Add(storyNum, items);
                }
                else
                {
                    AwailableStoryItemsID[storyNum].AddRange(items);
                }
            }
        }

        //AwailableStoryItemsID = 
        IsLoadedSeriesAwailableItems = true;
        DebugX.ColorMessage($"awailable items - {awailableItems}", Color.magenta);
    }
    #endregion



    #region Selected Items
    public void SaveSelectedItems()
    {
        SaveSelectedItems(StoryID, String.Join("\t", SelectedItemsID));
    }

    public void LoadSelectedItems()
    {
        IsLoadedSelectedItems = false;
        LoadSelectedItems(StoryID, SelectedItemsLoaded);
    }

    private void SelectedItemsLoaded(string selectedItems)
    {
        SelectedItemsID = selectedItems.Split('\t', StringSplitOptions.RemoveEmptyEntries);
        IsLoadedSelectedItems = true;
        DebugX.ColorMessage($"selected items - {selectedItems}", Color.magenta);
    }
    #endregion


    #region Selected Menus
    public void SaveSelectedMenus()
    {
        SaveSeriesSelectedMenu(StoryID, _series, String.Join("\t", SelectedMenus));
    }

    public void LoadSelectedMenus()
    {
        IsLoadedSeriesSelectedMenus = false;
        LoadSeriesSelectedMenu(StoryID, _series, SelectedMenusLoaded);
    }

    private void SelectedMenusLoaded(string selectedMenus)
    {
        SelectedMenus = new List<string>(selectedMenus.Split('\t', StringSplitOptions.RemoveEmptyEntries));
        IsLoadedSeriesSelectedMenus = true;
    }
    #endregion



    public void SaveSeriesSelectedItems(string[] storySelectedItems)
    {
        var data = string.Join("\t", storySelectedItems);
        SaveSeriesSelectedItems(StoryID, _series, data);
    }



    public void ResetSeriesData(Action callback)
    {
        ClearStoriesData(StoryID, _series, () =>
        {
            DOTween.Sequence().AppendInterval(1f).AppendCallback(() => callback?.Invoke());
        });

        _currentStoryData = String.Empty;

        if (_series > 0)
        {
            
            LoadSeriesSelectedItems(StoryID, _series - 1, selectedItems =>
            {
                if (string.Join("\t", SelectedItemsID) != selectedItems)
                {
                    SelectedItemsID = selectedItems.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    SaveSelectedItems();
                }
            });
        }
        else
        {
            SelectedItemsID = new string[0];
            SaveSelectedItems();
        }
    }


    public void ResetSeasonData(int startSeasonNum, Action callback)
    {
        _currentStoryData = String.Empty;

        for (int series = _series; series >= startSeasonNum; series--)
        {
            ClearStoriesData(StoryID, series, null);
        }

        if (startSeasonNum > 0)
        {
            LoadSeriesSelectedItems(StoryID, _series - 1, selectedItems =>
            {
                if (string.Join("\t", SelectedItemsID) != selectedItems)
                {
                    SelectedItemsID = selectedItems.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    SaveSelectedItems();
                    DOTween.Sequence().AppendInterval(1f).AppendCallback(() => callback?.Invoke());
                }
            });
        }
        else
        {
            SelectedItemsID = new string[0];
            SaveSelectedItems();
            DOTween.Sequence().AppendInterval(1f).AppendCallback(() => callback?.Invoke());
        }

        Series = startSeasonNum;
    }





    private void Log(string message, Color? color = null)
    {
        Debug.Log(DebugX.ColorPart($"[DataHandler]: ", Color.cyan) + DebugX.ColorPart(message, color == null ? Color.white : color.Value));
    }




















    #region db functions

    private const string _clearableDir = "data";
    private const string _constantDir = "const";


    #region Series num

    private string GetSeriesNumKey(string storyID) => $"{storyID}_series";


    private void SaveSeriesNum(string storyID, int seriesNum)
    {
        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getStoryDataReference(storyID).Child("data");
            dataRoot.Child("last_series_num").SetValueAsync(seriesNum);
            Log($"{_save} {_remote} SeriesNum {seriesNum}");
        }
        else
        {
            PlayerPrefs.SetInt(GetSeriesNumKey(storyID), seriesNum);
            Log($"{_save} {_local} SeriesNum {seriesNum}");
        }
    }

    private void LoadSeriesNum(string storyID, Action<int> callback)
    {
        //DebugX.ColorMessage($"Begin Load ({storyID}) last series Num ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);

        int lastSeriesNum = 0;

        if (_isLogginDelegate())
        {
            _getStoryDataReference(storyID).Child("data").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные истории" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Child("last_series_num").Exists)
                        lastSeriesNum = int.Parse(snapshot.Child("last_series_num").Value.ToString());
                }

                Log($"{_load} {_remote} SeriesNum {lastSeriesNum}");
                callback?.Invoke(lastSeriesNum);
            });
        }
        else
        {
            lastSeriesNum = PlayerPrefs.GetInt(GetSeriesNumKey(storyID), 0);

            Log($"{_load} {_local} SeriesNum {lastSeriesNum}");
            callback?.Invoke(lastSeriesNum);
        }
    }
    #endregion


    #region AwailableItems
    private string GetAwailableItemsKey(string storyID) => $"{storyID}_awailable_items";


    private void SaveAwailableItems(string storyID, string awailableItemsData)
    {
        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getStoryDataReference(storyID).Child("data").Child("awailable_items");
            dataRoot.SetValueAsync(awailableItemsData);
            Log($"{_save} {_remote} AwailableItemsData {awailableItemsData}");
        }
        else
        {
            PlayerPrefs.SetString(GetAwailableItemsKey(storyID), awailableItemsData);
            Log($"{_save} {_local} AwailableItemsData {awailableItemsData}");
        }
    }

    private void LoadAwailableItems(string storyID, Action<string> callback)
    {
        //DebugX.ColorMessage($"Begin Load ({storyID}) last series Num ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);

        string awailableItemsData = "";

        if (_isLogginDelegate())
        {
            _getStoryDataReference(storyID).Child("data").Child("awailable_items").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные истории" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                        awailableItemsData = snapshot.Value.ToString();
                }

                Log($"{_load} {_remote} AwailableItemsData {awailableItemsData}");
                callback?.Invoke(awailableItemsData);
            });
        }
        else
        {
            awailableItemsData = PlayerPrefs.GetString(GetAwailableItemsKey(storyID), "");

            Log($"{_load} {_local} AwailableItemsData {awailableItemsData}");
            callback?.Invoke(awailableItemsData);
        }
    }
    #endregion


    #region SelectedItems
    private string GetSelectedItemsKey(string storyID) => $"{storyID}_selected_items";


    private void SaveSelectedItems(string storyID, string selectedItemsData)
    {
        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getStoryDataReference(storyID).Child("data").Child("selected_items");
            dataRoot.SetValueAsync(selectedItemsData);
            Log($"{_save} {_remote} SelectedItems {selectedItemsData}");
        }
        else
        {
            PlayerPrefs.SetString(GetSelectedItemsKey(storyID), selectedItemsData);
            Log($"{_save} {_local} SelectedItems {selectedItemsData}");
        }
    }

    private void LoadSelectedItems(string storyID, Action<string> callback)
    {
        //DebugX.ColorMessage($"Begin Load ({storyID}) last series Num ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);

        string selectedItemsData = "";

        if (_isLogginDelegate())
        {
            _getStoryDataReference(storyID).Child("data").Child("selected_items").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные истории" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                        selectedItemsData = snapshot.Value.ToString();
                }

                Log($"{_load} {_remote} SelectedItems {selectedItemsData}");
                callback?.Invoke(selectedItemsData);
            });
        }
        else
        {
            selectedItemsData = PlayerPrefs.GetString(GetSelectedItemsKey(storyID), "");

            Log($"{_load} {_local} SelectedItems {selectedItemsData}");
            callback?.Invoke(selectedItemsData);
        }
    }
    #endregion


    #region SelectedMenu

    private string GetSeriesSelectedMenuKey(string storyID, int storyNum) => $"{storyID}_{storyNum}_selected_menus";

    private void SaveSeriesSelectedMenu(string storyID, int seriesNum, string selectedMenu)
    {
        if (!string.IsNullOrEmpty(selectedMenu))
        {
            if (_isLogginDelegate())
            {
                DatabaseReference dataRoot = _getSeriesDataReference(storyID, seriesNum).Child(_constantDir).Child("selected_menus");
                dataRoot.SetValueAsync(selectedMenu);
                Log($"{_save} {_remote} {storyID}/{seriesNum} SelectedMenu {selectedMenu}");
            }
            else
            {
                PlayerPrefs.SetString(GetSeriesSelectedMenuKey(storyID, seriesNum), selectedMenu);
                Log($"{_save} {_local} {storyID}/{seriesNum} SelectedMenu {selectedMenu}");
            }
        }
    }


    private void LoadSeriesSelectedMenu(string storyID, int seriesNum, Action<string> callback)
    {
        //DebugX.ColorMessage($"Begin Load Story Data ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);
        var selectedMenus = "";

        if (selectedMenus.IsNullOrEmpty() && _isLogginDelegate())
        {
            DatabaseReference dataRoot = _getSeriesDataReference(storyID, seriesNum).Child(_constantDir).Child("selected_menus");
            dataRoot.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists)
                    {
                        selectedMenus = snapshot.Value.ToString();
                    }
                }

                Log($"{_load} {_remote} {storyID}/{seriesNum} SelectedMenu {selectedMenus}");
                callback?.Invoke(selectedMenus);
            });
        }
        else
        {
            selectedMenus = PlayerPrefs.GetString(GetSeriesSelectedMenuKey(storyID, seriesNum), "");
            Log($"{_load} {_local} {storyID}/{seriesNum} SelectedMenu {selectedMenus}");
            callback?.Invoke(selectedMenus);
        }
    }
    #endregion


    #region StorySelectedItems
    private string GetSeriesSelectedItemsKey(string storyID, int storyNum) => $"{storyID}_{storyNum}_story_selected_items";


    private void SaveSeriesSelectedItems(string storyID, int storyNum, string storySelectedItemsData)
    {
        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getSeriesDataReference(storyID, storyNum).Child(_clearableDir).Child("selected_items");
            dataRoot.SetValueAsync(storySelectedItemsData);
            Log($"{_save} {_remote} STORY SelectedItems {storySelectedItemsData}");
        }
        else
        {
            PlayerPrefs.SetString(GetSeriesSelectedItemsKey(storyID, storyNum), storySelectedItemsData);
            Log($"{_save} {_local} STORY SelectedItems {storySelectedItemsData}");
        }
    }

    private void LoadSeriesSelectedItems(string storyID, int storyNum, Action<string> callback)
    {
        //DebugX.ColorMessage($"Begin Load ({storyID}) last series Num ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);

        string storySelectedItemsData = "";

        if (_isLogginDelegate())
        {
            _getSeriesDataReference(storyID, storyNum).Child(_clearableDir).Child("selected_items").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные истории" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                        storySelectedItemsData = snapshot.Value.ToString();
                }

                Log($"{_load} {_remote} STORY SelectedItems {storySelectedItemsData}");
                callback?.Invoke(storySelectedItemsData);
            });
        }
        else
        {
            storySelectedItemsData = PlayerPrefs.GetString(GetSeriesSelectedItemsKey(storyID, storyNum), "");

            Log($"{_load} {_local} STORY SelectedItems {storySelectedItemsData}");
            callback?.Invoke(storySelectedItemsData);
        }
    }
    #endregion



    #region  SeriesUnlocked

    private string GetSeriesUnlockedKey(string storyID) => $"{storyID}_is_unlocked";


    private void SaveSeriesUnlocked(string storyID, bool isAwailable)
    {
        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getStoryDataReference(storyID).Child("data");
            dataRoot.Child("is_unlocked").SetValueAsync(isAwailable);
            Log($"{_save} {_remote} awailable {isAwailable}", Color.green);
        }
        else
        {
            PlayerPrefs.SetInt(GetSeriesUnlockedKey(storyID), isAwailable ? 1 : 0);
            Log($"{_save} {_local} awailable {isAwailable}", Color.green);
        }
    }

    private void LoadSeriesUnlocked(string storyID, Action<bool> callback)
    {
        //DebugX.ColorMessage($"Begin Load ({storyID}) unlocked ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);

        bool isAwailable = false;

        if (_isLogginDelegate())
        {
            _getStoryDataReference(storyID).Child("data").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные истории" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Child("is_unlocked").Exists)
                    {
                        isAwailable = bool.Parse(snapshot.Child("is_unlocked").Value.ToString());
                    }
                }

                Log($"{_load} {_remote} awailable {isAwailable}", Color.green);
                callback?.Invoke(isAwailable);
            });
        }
        else
        {
            isAwailable = PlayerPrefs.GetInt(GetSeriesUnlockedKey(storyID), 0) == 1;

            Log($"{_load} {_local} awailable {isAwailable}", Color.green);
            callback?.Invoke(isAwailable);
        }
    }
    #endregion



    #region StoryData
    private void ClearStoriesData(string storyID, int seriesNum, Action callback)
    {
        SaveManager.Delete(GetFungusSaveKey(seriesNum));
        PlayerPrefs.DeleteKey(GetStoryVariableKey(storyID, seriesNum));
        PlayerPrefs.DeleteKey(GetSeriesSelectedItemsKey(storyID, seriesNum));

        if (_isLogginDelegate())
        {
            _getSeriesDataReference(storyID, seriesNum).Child(_clearableDir)
            .RemoveValueAsync()
            .ContinueWithOnMainThread(t =>
            {
                callback?.Invoke();
                Log($"{DebugX.ColorPart("Clear", Color.red)} {_remote} {storyID}/{seriesNum} data");
            });
        }
        else
        {
            callback?.Invoke();
            Log($"{DebugX.ColorPart("Clear", Color.red)} {_local} {storyID}/{seriesNum} data");
        }
    }


    private void SaveStoryDataToServer(string storyID, int seriesNum, string dataJSON)
    {
        if (!string.IsNullOrEmpty(dataJSON))
        {
            if (_isLogginDelegate())
            {
                DatabaseReference dataRoot = _getSeriesDataReference(storyID, seriesNum).Child(_clearableDir).Child("json_data");
                dataRoot.SetValueAsync(dataJSON);
                Log($"{_save} {_remote} {storyID}/{seriesNum} data ({dataJSON.Length * 2 / 1024f} kb)", Color.white);
            }
        }
    }


    private void LoadStoryData(string storyID, int seriesNum, Action<string, bool> callback)
    {
        //DebugX.ColorMessage($"Begin Load Story Data ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);
        var historyData = SaveManager.GetDataSaveHistory(GetFungusSaveKey(seriesNum));

        if (historyData.IsNullOrEmpty() && _isLogginDelegate())
        {
            DatabaseReference dataRoot = _getSeriesDataReference(storyID, seriesNum).Child(_clearableDir).Child("json_data");
            dataRoot.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists)
                    {
                        historyData = snapshot.Value.ToString();
                    }
                }

                Log($"{_load} {_remote} {storyID}/{seriesNum} data ({historyData.Length * 2 / 1024f} kb)", Color.yellow);
                callback?.Invoke(historyData, false);
            });
        }
        else
        {
            callback?.Invoke(historyData, true);
            Log($"{_load} {_local} {storyID}/{seriesNum} data ({historyData.Length * 2 / 1024f} kb)", Color.yellow);
        }
    }
    #endregion



    #region StoryVariables
    private string GetStoryVariableKey(string storyID, int seriesNum) => $"{storyID}/{seriesNum}/data";

    private void SaveStoryVariable(string storyID, int seriesNum, StoryData[] dataList)
    {
        string data = String.Join("\t", dataList.Select(d => d.Serialize()));

        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getSeriesDataReference(storyID, seriesNum).Child(_clearableDir).Child("variables");
            dataRoot.SetValueAsync(data);
            Log($"{_save} {_remote} {storyID}/{seriesNum} variables ({data.Length * 2 / 1024f} kb)", Color.magenta);
        }
        else
        {
            PlayerPrefs.SetString(GetStoryVariableKey(storyID, seriesNum), data);
            Log($"{_save} {_local} {storyID}/{seriesNum} variables ({data.Length * 2 / 1024f} kb)", Color.magenta);
        }
    }


    private void LoadStoryVariable(string storyID, int seriesNum, Action<StoryData[]> callback)
    {
        List<StoryData> datas = new List<StoryData>();

        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getSeriesDataReference(storyID, seriesNum).Child(_clearableDir).Child("variables");
            dataRoot.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists)
                    {
                        string[] variables = snapshot.Value.ToString().Split('\t', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var variable in variables)
                            datas.Add(StoryData.Deserialize(variable));
                    }
                }

                Log($"{_load} {_remote} {storyID}/{seriesNum} variables", Color.magenta);
                callback?.Invoke(datas.ToArray());
            });
        }
        else
        {
            string dataPath = GetStoryVariableKey(storyID, seriesNum);
            if (PlayerPrefs.HasKey(dataPath))
            {
                string[] variables = PlayerPrefs.GetString(dataPath).Split('\t', StringSplitOptions.RemoveEmptyEntries);

                foreach (var variable in variables)
                    datas.Add(StoryData.Deserialize(variable));
            }

            Log($"{_load} {_local} {storyID}/{seriesNum} variables", Color.magenta);
            callback?.Invoke(datas.ToArray());
        }
    }
    #endregion


    #region SeriesCompleted

    private string GetSeriesCompletedKey(string storyID) => $"{storyID}/completed";


    private void SaveSeriesCompleted(string storyID, int completedSeries)
    {
        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getStoryDataReference(storyID).Child("data");
            dataRoot.Child("completed").SetValueAsync(completedSeries);
            Log($"{_save} {_remote} Series complete {completedSeries}");
        }
        else
        {
            PlayerPrefs.SetInt(GetSeriesCompletedKey(storyID), completedSeries);
            Log($"{_save} {_local} Series complete {completedSeries}");
        }
    }

    private void LoadSeriesCompleted(string storyID, Action<int> callback)
    {
        //DebugX.ColorMessage($"Begin Load Story Data ({(_auth.IsLoggin ? "authorized" : "NOT authorized")})", Color.yellow);
        int completedSeries = 0;

        if (_isLogginDelegate())
        {
            DatabaseReference dataRoot = _getStoryDataReference(storyID).Child("data");
            dataRoot.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Не получилось прочитать данные" + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Child("completed").Exists)
                        completedSeries = int.Parse(snapshot.Child("completed").Value.ToString());
                }

                Log($"{_load} {_remote} Series complete {completedSeries}");
                //DebugX.ColorMessage($"Complete Load {storyID} completed", Color.yellow);
                callback?.Invoke(completedSeries);
            });
        }
        else
        {
            completedSeries = PlayerPrefs.GetInt(GetSeriesCompletedKey(storyID), 0);
            Log($"{_load} {_local} Series complete {completedSeries}");
            callback?.Invoke(completedSeries);
        }
    }
    #endregion



    #endregion
}