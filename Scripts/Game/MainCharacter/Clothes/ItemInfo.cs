using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum SelectableItemType
{
    Makeup,
    Dress,
    Hair,
    Skin,
}

public interface ISelectableInfo
{
    public string GetName();
    public string GetID();
    public bool GetIsCreated();
    public SelectableItemType GetItemType();
}

[Serializable]
public struct ItemInfo : ISelectableInfo
{
    [SerializeField] string itemName;
    [SerializeField] ItemType itemType;
    [SerializeField] string itemID;
    [SerializeField] int itemPrice;
    [SerializeField] Sprite itemSprite;

    public Sprite Sprite => itemSprite;
    public ItemType Type => itemType;
    public int Price => itemPrice;


    public static ItemInfo Empty()
    {
        ItemInfo empty = new ItemInfo();
        empty.itemName = "None";
        empty.itemID = "";
        empty.itemType = ItemType.Makeup;
        return empty;
    }


    public static bool operator ==(ItemInfo obj1, ItemInfo obj2)
    {
        return obj1.itemType == obj2.itemType && obj1.itemName == obj2.itemName && obj1.itemID == obj2.itemID && obj1.itemSprite == obj2.itemSprite;
    }

    public static bool operator !=(ItemInfo obj1, ItemInfo obj2)
    {
        return obj1.itemType != obj2.itemType || obj1.itemName != obj2.itemName || obj1.itemID != obj2.itemID || obj1.itemSprite != obj2.itemSprite;
    }

    public override bool Equals(object obj)
    {
        ItemInfo o = (obj as ItemInfo?).Value;
        return o.itemType == itemType && o.itemName == itemName && o.itemID == itemID && o.itemSprite == itemSprite;
    }

/*
    public override int GetHashCode()
    {
        int hash = itemName.Select(l => (int)l).Sum();
        return base.GetHashCode();
    }
*/
    public string GetName() => itemName;

    public string GetID() => $"item_{itemType}_{itemID}";

    public SelectableItemType GetItemType() => (SelectableItemType)itemType;

    public bool GetIsCreated() => itemSprite != null && itemID != string.Empty;
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ItemInfo))]
public class ItemInfoPropertyDrawer : PropertyDrawer
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

    private void DrawMainProperties(Rect rect, SerializedProperty human)
    {
        rect.width = (rect.width - 3 * space) / 5;
        DrawStringPlaceholderField(rect, human.FindPropertyRelative("itemName"), "имя");
        rect.x += rect.width + space;
        DrawStringPlaceholderField(rect, human.FindPropertyRelative("itemID"), "ID предмета (обязательно уникально)", "(ID)");
        rect.x += rect.width + space;
        DrawProperty(rect, human.FindPropertyRelative("itemType"));
        rect.x += rect.width + space;
        DrawPricePlaceholderField(rect, human.FindPropertyRelative("itemPrice"));
        rect.x += rect.width + space;
        DrawProperty(rect, human.FindPropertyRelative("itemSprite"));
    }

    private void DrawProperty(Rect rect, SerializedProperty property)
    {
        EditorGUI.PropertyField(rect, property, GUIContent.none);
    }

    private void DrawLabel(Rect rect, string name)
    {
        EditorGUI.LabelField(rect, name);
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


    private void DrawPricePlaceholderField(Rect rect, SerializedProperty property)
    {
        var placeholderTextStyle = new GUIStyle(EditorStyles.textArea);
        placeholderTextStyle.fontStyle = FontStyle.Italic;

        int value = property.intValue;

        string textVal = EditorGUI.TextArea(rect, value.ToString());

        if (!int.TryParse(textVal, out value) || value <= 0)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextArea(rect, "        (бесплатно)", placeholderTextStyle);
            EditorGUI.EndDisabledGroup();
            value = 0;
        }

        property.intValue = value;
    }
}
#endif