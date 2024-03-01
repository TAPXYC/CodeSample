using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using Fungus;




#if FBON
using Firebase.Database;
using Firebase.Extensions;
#endif



public class DBController
{
    public readonly IntegerHandler DiamondHandler;
    public readonly IntegerHandler CardHandler;

    public readonly IntegerHandler LastBonusRewardHandler;
    public readonly StringHandler LastBonusRewardTimeHandler;
    public readonly IntegerHandler LastRewardHandler;
    public readonly StringHandler LastRewardTimeHandler;

    public bool IsLoaded
    {
        get;
        private set;
    }

    private List<StoryDataHandler> _storyDataHandlers;
    private MonoBehaviour _owner;
    private AuthController _auth;
    private DatabaseReference _db;
    private Coroutine _updateCoroutine;

    public DBController(MonoBehaviour owner, AuthController authController)
    {
        _storyDataHandlers = new List<StoryDataHandler>();
        _owner = owner;

        _auth = authController;
        _auth.OnCompleteLogin += (message, userData) => UpdateUserData();
        _auth.OnDeleteAccount += DeleteData;

        _db = FirebaseDatabase.DefaultInstance.RootReference;
        DiamondHandler = new IntegerHandler("diamonds", 100, SaveIntResource, LoadIntResource);
        CardHandler = new IntegerHandler("cards", 2, SaveIntResource, LoadIntResource);
        LastBonusRewardHandler = CreateIntegerHandler("bonus_rew", 0);
        LastBonusRewardTimeHandler = CreateStringHandler("bonus_rew_t", "");
        LastRewardHandler = CreateIntegerHandler("rew", 0);
        LastRewardTimeHandler = CreateStringHandler("rew_t", "");
        IsLoaded = false;
    }



    public IntegerHandler CreateIntegerHandler(string name, int defaultValue)
    {
        return new IntegerHandler(name, defaultValue, SaveIntResource, LoadIntResource);
    }

    public StringHandler CreateStringHandler(string name, string defaultValue)
    {
        return new StringHandler(name, defaultValue, SaveStringResource, LoadStringResource);
    }


    public StoryDataHandler AddStoryDataHandler(string storyID)
    {
        StoryDataHandler storyDataHandler = new StoryDataHandler(storyID, IsLogginMode, GetStoryDataReference, GetSeriesDataReference);

        _storyDataHandlers.Add(storyDataHandler);
        return storyDataHandler;
    }

    public StoryDataHandler GetStoryDataHandler(string storyID)
    {
        return _storyDataHandlers.FirstOrDefault(s => s.StoryID == storyID);
    }


    public void DeleteData()
    {
        if (_auth.IsLoggin)
        {
            _db.Child("users").Child(_auth.AuthUserData.ID).RemoveValueAsync();
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
    }



    public void UpdateUserData()
    {
        if (_updateCoroutine != null)
            _owner.StopCoroutine(_updateCoroutine);

        _updateCoroutine = _owner.StartCoroutine(LoadCoroutine());
    }


    private IEnumerator LoadCoroutine()
    {
        IsLoaded = false;

        DiamondHandler.Load();
        CardHandler.Load();
        LastBonusRewardHandler.Load();
        LastBonusRewardTimeHandler.Load();
        LastRewardHandler.Load();
        LastRewardTimeHandler.Load();

        foreach (var storyData in _storyDataHandlers)
            storyData.LoadAll();

        yield return new WaitUntil(() => DiamondHandler.IsLoaded);
        yield return new WaitUntil(() => CardHandler.IsLoaded);
        yield return new WaitUntil(() => LastBonusRewardHandler.IsLoaded);
        yield return new WaitUntil(() => LastBonusRewardTimeHandler.IsLoaded);
        yield return new WaitUntil(() => LastRewardHandler.IsLoaded);
        yield return new WaitUntil(() => LastRewardTimeHandler.IsLoaded);

        foreach (var storyData in _storyDataHandlers)
            yield return new WaitUntil(() => storyData.IsLoaded);

        _updateCoroutine = null;
        IsLoaded = true;
    }




    #region DB references


    private bool IsLogginMode() => _auth.IsLoggin;

    private DatabaseReference GetUserDataReference()
    {
        return _db.Child("users").Child(_auth.AuthUserData.ID).Child("data");
    }

    private DatabaseReference GetStoryDataReference(string storyID)
    {
        return _db.Child("users").Child(_auth.AuthUserData.ID).Child(storyID);
    }
    private DatabaseReference GetSeriesDataReference(string storyID, int seriesNum)
    {
        return GetStoryDataReference(storyID).Child($"s{seriesNum}");
    }
    #endregion




    #region Resources
    public void SaveIntResource(string name, int value)
    {
        if (_auth.IsLoggin)
        {
            GetUserDataReference().Child(name).SetValueAsync(value);
        }
        else
        {
            PlayerPrefs.SetInt(name, value);
        }
    }

    public void LoadIntResource(string name, Action<int> callback, int defaultValue = 0)
    {
        int diamonds = 0;

        if (_auth.IsLoggin)
        {
            GetUserDataReference().Child(name).GetValueAsync().ContinueWithOnMainThread(task =>
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
                        diamonds = int.Parse(snapshot.Value.ToString());
                    }
                    else
                    {
                        diamonds = defaultValue;
                        SaveIntResource(name, defaultValue);
                    }
                }

                callback?.Invoke(diamonds);
            });
        }
        else
        {
            diamonds = PlayerPrefs.GetInt(name, defaultValue);
            callback?.Invoke(diamonds);
        }
    }


    public void SaveStringResource(string name, string value)
    {
        if (_auth.IsLoggin)
        {
            GetUserDataReference().Child(name).SetValueAsync(value);
        }
        else
        {
            PlayerPrefs.SetString(name, value);
        }
    }

    public void LoadStringResource(string name, Action<string> callback, string defaultValue = "")
    {
        string data = "";

        if (_auth.IsLoggin)
        {
            GetUserDataReference().Child(name).GetValueAsync().ContinueWithOnMainThread(task =>
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
                        data = snapshot.Value.ToString();
                    }
                    else
                    {
                        data = defaultValue;
                        SaveStringResource(name, defaultValue);
                    }
                }

                callback?.Invoke(data);
            });
        }
        else
        {
            data = PlayerPrefs.GetString(name, defaultValue);
            callback?.Invoke(data);
        }
    }
    #endregion







}
