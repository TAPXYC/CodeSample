using System;
using System.Linq;
using Fungus;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[Serializable]
public struct Stat
{
    [SerializeField] string name;
    [SerializeField] IntegerData value;
    [Tooltip("Показывать параметр в окне статов")]
    [SerializeField] bool isVisible;
    [Tooltip("Будет ли этот параметр передаваться в другую историю")]
    [SerializeField] bool sendToNextStory;

    public string Name => name;
    public IntegerData Value => value;
    public bool IsVisible => isVisible;
    public bool SendToNextStory => sendToNextStory;

    public StoryData GetStoryData()
    {
        Variable variable = value.integerRef;
        return new StoryData(variable.Key, variable.GetValue().ToString(), StoryData.GetDataType(variable.GetType()));
    }
}



[CommandInfo("Команды", "Создать характеристики", "Инициализирует названия и значения характеристик")]
public class SetStatsCommand : Command
{
    [SerializeField] StringData characterName;
    [Header("(Имя)   (Переменная)   (Видна в окне статов)    (Передается в другую историю)")]
    [SerializeField] Stat[] stats;

    public override void OnEnter()
    {
        GameManager.Story.StoryStage.MainCharacter.SetStats(stats, characterName);
        Continue();
    }

    public override string GetSummary()
    {
        return $"Инициализация статов: {(stats.IsNullOrEmpty() ? "" : string.Join(", ", stats.Select(s => s.Name)))}";
    }

    public override Color GetButtonColor()
    {
        return new Color(1, 1, 1);
    }
}








#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Stat))]
public class StatDrawler : PropertyDrawer
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

    private void DrawMainProperties(Rect rect, SerializedProperty prop)
    {
        rect.width = (rect.width - 2 * space) / 6;
        DrawStringPlaceholderField(rect, prop.FindPropertyRelative("name"), "имя");
        rect.x += rect.width + space;
        EditorGUI.PropertyField(rect, prop.FindPropertyRelative("value"), GUIContent.none);
        rect.x += rect.width + space;
        EditorGUI.PropertyField(rect, prop.FindPropertyRelative("isVisible"), GUIContent.none);
        rect.x += rect.width + space;
        EditorGUI.PropertyField(rect, prop.FindPropertyRelative("sendToNextStory"), GUIContent.none);
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