using System.Linq;
using System.Collections.Generic;
using Fungus.EditorUtils;
using UnityEditor;
using UnityEngine;
using Fungus;
using System;

[CustomEditor(typeof(InitPersonCommand))]
public class InitPersonCommandEditor : CommandEditor
{
    protected SerializedProperty messageProp;
    protected SerializedProperty itemCollectionProp;
    protected SerializedProperty startItemsIdProp;
    protected SerializedProperty selectItemScreenBlockProp;

    private GUIContent moveButtonContent = new GUIContent("\u21b4", "Переместить ниже");
    private GUIContent deleteButtonContent = new GUIContent("-", "удалить");
    private GUIContent addButtonContent = new GUIContent("+", "Добавить");
    private GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    public override void OnEnable()
    {
        base.OnEnable();
        messageProp = serializedObject.FindProperty("message");
        itemCollectionProp = serializedObject.FindProperty("itemCollection");
        startItemsIdProp = serializedObject.FindProperty("startItemsId");
        selectItemScreenBlockProp = serializedObject.FindProperty("selectItemScreen");
    }

    public override void DrawCommandGUI()
    {
        var flowchart = FlowchartWindow.GetFlowchart();
        if (flowchart == null)
        {
            return;
        }

        serializedObject.Update();

        EditorGUILayout.PropertyField(messageProp);
        EditorGUILayout.PropertyField(selectItemScreenBlockProp);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(itemCollectionProp);

        ItemCollection itemCollection = itemCollectionProp.GetValue() as ItemCollection;

        if (itemCollection != null)
        {
            ShowStartItemList(startItemsIdProp, itemCollection.CollectionSnapshot);
        }
        else
        {
            startItemsIdProp.ClearArray();
            EditorGUILayout.HelpBox("Выберите коллекцию предметов, из которой будет производиться выбор", MessageType.Error);
        }

        serializedObject.ApplyModifiedProperties();
    }





    private void ShowStartItemList(SerializedProperty list, ItemInfo[] elements)
    {
        SerializedProperty size = list.FindPropertyRelative("Array.size");
        if (size.intValue == 0)
        {
            EditorGUILayout.HelpBox("Выберите начальные предметы (опционально)", MessageType.Info);
        }
        else
        {
            var conflictedMessage = GetSelectedItemsFromID(list, elements).Where(i => i.GetIsCreated())
                                                                    .GroupBy(i => i.Type)
                                                                    .Select(g => g.Count() > 1 ? $"{g.Key} ({string.Join(", ", g.Select(i => i.GetName()))})" : "")
                                                                    .Where(m => m != "")
                                                                    .ToArray();
            if (conflictedMessage.Length == 0)
            {
                EditorGUILayout.HelpBox("Начальные предметы", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"ВНИМАНИЕ! Среди начальных предметов есть предметы одной категории: {string.Join(", ", conflictedMessage)}.\nБудет применен только последний выбранный элемент из каждой группы", MessageType.Warning);
            }
        }

        if (size.hasMultipleDifferentValues)
        {
            EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
        }
        else
        {
            ShowElements(list, elements);
        }
    }


    private void ShowElements(SerializedProperty list, ItemInfo[] elements)
    {
        elements = elements.Append(ItemInfo.Empty()).OrderBy(i => i.Type).ThenBy(i => i.GetIsCreated()).ToArray();
        var elementsName = GenerateItemNames(list, elements);
        var nowSelected = GetSelectedItemsFromID(list, elements);

        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string itemID = list.GetArrayElementAtIndex(i).stringValue;
            ItemInfo item = elements.FirstOrDefault(e => e.GetID() == itemID);

            if (item.GetIsCreated() || item.GetID() == ItemInfo.Empty().GetID())
            {
                int selectedIndex = IndexOf(elements, item);
                selectedIndex = EditorGUILayout.Popup(selectedIndex, elementsName);

                string selectedID = elements[selectedIndex].GetID();

                if (!nowSelected.Any(i => i.GetID() == selectedID))
                    list.GetArrayElementAtIndex(i).stringValue = selectedID;

                ShowButtons(list, i);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                list.DeleteArrayElementAtIndex(i);
                i--;
            }
        }

        if (GUILayout.Button(addButtonContent, EditorStyles.miniButton))
        {
            int lastIndex = list.arraySize;
            list.InsertArrayElementAtIndex(lastIndex);
            list.GetArrayElementAtIndex(lastIndex).stringValue = elements[0].GetID();
        }
    }





    private string[] GenerateItemNames(SerializedProperty list, ItemInfo[] elements)
    {
        var elementList = new List<string>();
        var selectedItems = GetSelectedItemsFromID(list, elements);

        foreach (var item in elements)
        {
            elementList.Add(item.GetIsCreated() ? $"({item.Type}) {item.GetName()} {(selectedItems.Any(e => e.GetID() == item.GetID()) ? "(\u2713)" : "")}" :
                                                    "None");
        }

        return elementList.ToArray();
    }


    private List<ItemInfo> GetSelectedItemsFromID(SerializedProperty list, ItemInfo[] elements)
    {
        var selectedItems = new List<ItemInfo>();

        for (int i = 0; i < list.arraySize; i++)
        {
            string id = list.GetArrayElementAtIndex(i).stringValue;
            if (elements.Any(e => e.GetID() == id))
            {
                selectedItems.Add(elements[IndexOf(elements, id)]);
            }
        }
        return selectedItems;
    }


    private int IndexOf(ItemInfo[] elements, ItemInfo item)
    {
        return IndexOf(elements, item.GetID());
    }

    private int IndexOf(ItemInfo[] elements, string id)
    {
        int index = 0;
        for (; index < elements.Length; index++)
        {
            if (elements[index].GetID() == id)
            {
                break;
            }
        }

        return index;
    }

    private void ShowButtons(SerializedProperty list, int index)
    {
        if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            list.MoveArrayElement(index, index + 1);
        }
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            int oldSize = list.arraySize;
            list.DeleteArrayElementAtIndex(index);
            if (list.arraySize == oldSize)
            {
                list.DeleteArrayElementAtIndex(index);
            }
        }
    }
}