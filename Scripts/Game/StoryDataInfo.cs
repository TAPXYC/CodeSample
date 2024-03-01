using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SeriesInfo
{
    [SerializeField] string chapterName;
    [SerializeField] AssetReferenceGameObject storyPrefabAsset;

    public string ChapterName => chapterName;

    public GameObject StoryGO
    {
        get;
        private set;
    }

    public async void LoadStoryPrefab(Action<StoryController> onComplete)
    {
        AsyncOperationHandle<GameObject> asyncOperationHandle = storyPrefabAsset.LoadAssetAsync();
        await asyncOperationHandle.Task;
        onComplete?.Invoke(asyncOperationHandle.Result.GetComponent<StoryController>());
    }


    public AsyncOperationHandle<GameObject>? LoadStoryPrefab()
    {
        if (StoryGO == null)
        {
            AsyncOperationHandle<GameObject> ao = storyPrefabAsset.LoadAssetAsync();
            LoadingStoryGO(ao);
            return ao;
        }
        else return null;
    }


    public void ReleaseStoryPrefab()
    {
        storyPrefabAsset.ReleaseAsset();
    }

    private async void LoadingStoryGO(AsyncOperationHandle<GameObject> ao)
    {
        await ao.Task;
        StoryGO = ao.Result;
    }
}

[CreateAssetMenu]
public class StoryDataInfo : ScriptableObject
{
    [Header("Общее")]
    [Tooltip("Отображаемое название истории")]
    [SerializeField] string storyName;
    [Tooltip("Идентификатор истории. ДОЛЖЕН БЫТЬ УНИКАЛЕН ДЛЯ ВСЕХ ИСТОРИЙ. Не виден пользователям")]
    [SerializeField] string storyID;
    [Tooltip("Таги истории")]
    [SerializeField] string[] tags;
    [Tooltip("Описание истории")]
    [SerializeField] string decription;
    [Space]
    [Header("Меню")]
    [Tooltip("Название в виде зарендериного шрифта")]
    [SerializeField] Sprite header;
    [Tooltip("Изображение на обложке")]
    [SerializeField] Sprite image;
    [Tooltip("Фон при предпросмотре истории в меню")]
    [SerializeField] Sprite background;
    [Space]
    [Header("Экран загрузки")]
    [Tooltip("Фон при предпросмотре истории в меню")]
    [SerializeField] Sprite[] loadingBackgrounds;
    [Tooltip("Интервал смены изображений")]
    [SerializeField] float changeInterval = 1;
    [Tooltip("Время смены картинки")]
    [SerializeField] float fadeTime = 1;
    [Space]
    [Header("Экран окончания")]
    [Tooltip("Фон при предпросмотре истории в меню")]
    [SerializeField] Sprite endScreenBackground;
    [Space]
    [Header("Контент истории")]
    [SerializeField] Season[] seasons;
    [SerializeField] AssetReferenceT<ItemCollection> itemsAsset;

    public bool CompleteStory => _storyDataHandler.Series >= TotalSeriesCount;
    public StoryDataHandler StoryDataHandler => _storyDataHandler;
    public string StoryName => storyName;
    public string StoryID => storyID;
    public string Decription => decription;
    public string[] Tags => tags;
    public Sprite Image => image;
    public Sprite Header => header;
    public Sprite Background => background;
    public int TotalSeriesCount => AllSeries.Length;
    public SeriesInfo[] AllSeries => seasons.Select(s => s.Series).SelectMany(s => s).ToArray();
    public Sprite[] LoadingBackgrounds => loadingBackgrounds;
    public Sprite EndScreenBackground => endScreenBackground;
    public float ChangeInterval => changeInterval;
    public float FadeTime => fadeTime;

    public ItemCollection Items
    {
        get;
        private set;
    }

    /// <summary>
    /// Номер сезона от 0
    /// </summary> 
    public int CurrentSeason
    {
        get
        {
            int currentSeriesNum = Mathf.Clamp(_storyDataHandler.Series, 0, TotalSeriesCount - 1);

            int season = 0;
            int endSeasonSeries = seasons[season].SeriesCount;

            while (endSeasonSeries <= currentSeriesNum)
            {
                season++;
                endSeasonSeries += seasons[season].SeriesCount;
            }

            return season;
        }
    }

    /// <summary>
    /// Номер серии от 0
    /// </summary> 
    public int SeriesNumber
    {
        get
        {
            int currentSeriesNum = Mathf.Clamp(_storyDataHandler.Series, 0, TotalSeriesCount - 1); ;
            int lastSeasonsSeriesCount = 0;

            for (int i = 0; i < CurrentSeason; i++)
                lastSeasonsSeriesCount += seasons[i].SeriesCount;

            return currentSeriesNum - lastSeasonsSeriesCount;
        }
    }

    public bool Unlocked => _storyDataHandler.SeriesUnlocked;

    public int SeasonSeriesCount => seasons[CurrentSeason].SeriesCount;

    public SeriesInfo CurrentSeriesInfo => AllSeries[Mathf.Clamp(_storyDataHandler.Series, 0, TotalSeriesCount - 1)];


    private StoryDataHandler _storyDataHandler;
    private DBController _db;

    public void Init(DBController db)
    {
        _db = db;
        _storyDataHandler = _db.AddStoryDataHandler(storyID);
    }

    public void SetUnlocked(bool unlocked)
    {
        _storyDataHandler.SeriesUnlocked = unlocked;
    }

    public void ResetSeason(Action onComplete)
    {
        int beginSeasonSeries = 0;

        for (int i = 0; i < CurrentSeason; i++)
        {
            beginSeasonSeries += seasons[CurrentSeason].SeriesCount;
        }
        _storyDataHandler.ResetSeasonData(beginSeasonSeries, onComplete);
    }


    public void ResetSeries(Action onComplete)
    {
        if (_storyDataHandler.Series < TotalSeriesCount)
            _storyDataHandler.ResetSeriesData(onComplete);
        else
        {
            _storyDataHandler.ResetSeasonData(TotalSeriesCount - 1, onComplete);
        }
    }



    public void NextStory()
    {
        _storyDataHandler.Series++;
        _storyDataHandler.SeriesUnlocked = false;
    }


    public AsyncOperationHandle<ItemCollection>? LoadItems()
    {
        if (Items == null)
        {
            AsyncOperationHandle<ItemCollection> handle = itemsAsset.LoadAssetAsync();
            WaitItemLoaded(handle);
            return handle;
        }
        else
            return null;
    }


    public void ReleaseItems()
    {
        itemsAsset.ReleaseAsset();
    }

    private async void WaitItemLoaded(AsyncOperationHandle<ItemCollection> handle)
    {
        await handle.Task;
        Items = handle.Result;
    }



    [Serializable]
    private struct Season
    {
        [SerializeField] SeriesInfo[] seriesInfo;
        public SeriesInfo[] Series => seriesInfo;
        public int SeriesCount => seriesInfo.Length;
    }
}





#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SeriesInfo))]
public class SeriesInfoPropertyDrawer : PropertyDrawer
{

    private const float space = 5;

    public override void OnGUI(Rect rect,
                               SerializedProperty property,
                               GUIContent label)
    {
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var firstLineRect = new Rect(
            x: rect.x,
            y: rect.y,
            width: rect.width,
            height: EditorGUIUtility.singleLineHeight
        );
        DrawMainProperties(firstLineRect, property);

        EditorGUI.indentLevel = indent;
    }

    private void DrawMainProperties(Rect rect, SerializedProperty serializeObject)
    {
        rect.width = (rect.width - 3 * space) / 2;
        DrawStringPlaceholderField(rect, serializeObject.FindPropertyRelative("chapterName"), "имя");
        rect.x += rect.width + space;
        EditorGUI.PropertyField(rect, serializeObject.FindPropertyRelative("storyPrefabAsset"), GUIContent.none);
    }

    private void DrawStringPlaceholderField(Rect rect, SerializedProperty property, string placeholder, string correctPlaceholder = "")
    {
        var placeholderTextStyle = new GUIStyle(EditorStyles.textArea);
        placeholderTextStyle.fontStyle = FontStyle.Italic;

        string text = property.stringValue;

        text = EditorGUI.TextArea(rect, text);

        if (string.IsNullOrEmpty(text))
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextArea(rect, placeholder, placeholderTextStyle);
            EditorGUI.EndDisabledGroup();
        }

        property.stringValue = text;
    }
}
#endif