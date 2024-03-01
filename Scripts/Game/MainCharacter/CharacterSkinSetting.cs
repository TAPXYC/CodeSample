using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EmotionType
{
    Normal,
    Sad,
    Smile,
    Confused,
    Disgruntled,
    Eyes,
    Surprised,
    Young,
    Custom
}


[Serializable]
public struct CharacterEmotion
{
    [SerializeField] EmotionType emotion;
    [SerializeField] Sprite sprite;
    [SerializeField] string emotionID;
    [SerializeField] bool hideItems;
    [SerializeField] bool hideHair;

    public EmotionType Emotion => emotion;
    public Sprite Sprite => sprite;
    public string EmotionID => emotionID;
    public bool IsCreated => sprite != null;
    public bool HideItems => hideItems;
    public bool HideHair => hideHair;
}



[CreateAssetMenu]
public class CharacterSkinSetting : ScriptableObject, ISelectableInfo
{
    [SerializeField] string skinName;
    [SerializeField] string skinID;
    [Header("(Тип эмоции)   (Спрайт эмоции)   (Скрывать одежду)    (Скрывать волосы)")]
    [SerializeField] CharacterEmotion[] emotions;

    public string SkinName => skinName;
    public string SkinID => skinID;

    public string GetID() => "skin_" + skinID;

    public SelectableItemType GetItemType() => SelectableItemType.Skin;

    public string GetName() => skinName;
    public bool GetIsCreated() => !emotions.IsNullOrEmpty();

    public Sprite GetSkin(EmotionType emotionType, out bool hideItems, out bool hideHair)
    {
        return GetSkin(emotionType.ToString(), emotions.FirstOrDefault(e => e.Emotion == emotionType), out hideItems, out hideHair);
    }

    public Sprite GetSkin(string emotionID, out bool hideItems, out bool hideHair)
    {
        return GetSkin(emotionID, emotions.FirstOrDefault(e => e.EmotionID == emotionID), out hideItems, out hideHair);
    }


    private Sprite GetSkin(string emotionName, CharacterEmotion emotion, out bool hideItems, out bool hideHair)
    {
        if (emotion.IsCreated)
        {
            hideItems = emotion.HideItems;
            hideHair = emotion.HideHair;
            return emotion.Sprite;
        }
        else
        {
            hideItems = false;
            hideHair = false;
            Debug.LogError($"Не задана эмоция {emotionName} на {skinName}");
            return emotions[0].Sprite;
        }
    }
}






#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CharacterEmotion))]
public class CharacterEmotionPropertyDrawer : PropertyDrawer
{

    private const float space = 5;

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
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

    private void DrawMainProperties(Rect rect, SerializedProperty emotion)
    {
        var emotionType = emotion.FindPropertyRelative("emotion");
        bool isCustom = (EmotionType)(emotionType.enumValueIndex) == EmotionType.Custom;

        rect.width = (rect.width - 3 * space) / (isCustom ? 5 : 4);
        DrawProperty(rect, emotionType);

        if (isCustom)
        {
            rect.x += rect.width + space;
            DrawStringPlaceholderField(rect, emotion.FindPropertyRelative("emotionID"), "ID эмоции (обязательно уникально)", "(ID)");
        }

        rect.x += rect.width + space;
        DrawProperty(rect, emotion.FindPropertyRelative("sprite"));
        rect.x += rect.width + space;
        DrawProperty(rect, emotion.FindPropertyRelative("hideItems"));
        rect.x += rect.width + space;
        DrawProperty(rect, emotion.FindPropertyRelative("hideHair"));
    }

    private void DrawProperty(Rect rect, SerializedProperty property)
    {
        EditorGUI.PropertyField(rect, property, GUIContent.none);
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
        /*
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            int spacesCount = Mathf.Clamp(text.Length, 6, 12);
            string spaceStr = new string(Enumerable.Range(0, spacesCount).Select(i => ' ').ToArray());
            EditorGUI.TextArea(rect, spaceStr + correctPlaceholder);
            EditorGUI.EndDisabledGroup();
        }*/

        property.stringValue = text;
    }
}
#endif