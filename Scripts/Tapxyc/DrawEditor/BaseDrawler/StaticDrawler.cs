#if UNITY_EDITOR
namespace Tapxyc.Drawler
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using TapxycRectEx;



    public static class StaticDrawler
    {
        #region Draw elements methods

        #region Label

        /// <summary>Draws a simple label in the inspector window</summary>
        /// <param name="text">Displayed text</param>
        public static void PrefixLabel(string text)
        {
            EditorGUILayout.PrefixLabel(text);
        }



        /// <summary>Draws a simple label in the inspector window</summary>
        /// <param name="text">Displayed text</param>
        /// <param name="labelStyle">Text style</param>
        public static void PrefixLabel(string content, GUIStyle labelStyle)
        {
            EditorGUILayout.PrefixLabel(content, labelStyle, labelStyle);
        }




        /// <summary>Draws a simple label in the inspector window</summary>
        /// <param name="text">Displayed text</param>
        /// <param name="labelStyle">Text style</param>
        public static void PrefixLabel(string content, Rect position, GUIStyle labelStyle)
        {
            EditorGUI.PrefixLabel(position, new GUIContent(content), labelStyle);
        }




        /// <summary>Draws a simple label in the inspector window that can be copied</summary>
        /// <param name="text">Displayed text</param>
        public static void CopyEnabledLabel(string text)
        {
            EditorGUILayout.SelectableLabel(text);
        }



        /// <summary>Draws a simple label in the inspector window that can be copied</summary>
        /// <param name="text">Displayed text</param>
        public static void CopyEnabledLabel(string text, GUIStyle style)
        {
            EditorGUILayout.SelectableLabel(text, style);
        }


        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        public static void Label(string value) => EditorGUILayout.LabelField(value);



        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        public static void Label(string value, GUIStyle style) => EditorGUILayout.LabelField(value, style);


        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        public static void Label(string name, string value) => EditorGUILayout.LabelField(name, value);


        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        public static void Label(string name, string value, GUIStyle style) => EditorGUILayout.LabelField(name, value, style);




        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>    
        public static void Label(string name, Texture value)
        {
            var contentName = new GUIContent(name);
            var contentTexture = new GUIContent(value);

            EditorGUILayout.LabelField(contentName, contentTexture);
        }

        #endregion





        #region Switcher

        /// <summary>Draws a button in the inspector window</summary>
        /// <param name="content">The inscription on the button</param>
        ///<return>Whether the button was pressed or not</return>
        public static bool Button(string content)
        {
            return GUI.Button(EditorGUILayout.GetControlRect(false), content);
        }

        /// <summary>Draws a button in the inspector window</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="buttonStyle">Button Style</param>
        ///<return>Whether the button was pressed or not</return>
        public static bool Button(string content, GUIStyle buttonStyle)
        {
            return GUI.Button(EditorGUILayout.GetControlRect(false), content, buttonStyle);
        }

        #endregion





        #region Property

        #region object

        /// <summary>
        /// ������������ ���� UnityEngine.Object � ����������
        /// </summary>
        /// <typeparam name="T">��� ������� ����</typeparam>
        /// <param name="gameObject">��� ������</param>
        /// <param name="isOnScene">����� �� ���������������� ��������� �� �����</param>
        /// <returns>������������� ����</returns>
        public static object ObjectField<T>(T gameObject, bool isOnScene) where T : UnityEngine.Object
        {
            return EditorGUILayout.ObjectField(gameObject, typeof(T), isOnScene);
        }


        /// <summary>
        /// ������������ ���� UnityEngine.Object � ����������
        /// </summary>
        /// <typeparam name="T">��� ������� ����</typeparam>
        /// <param name="name">������������ ��� ����</param>
        /// <param name="gameObject">��� ������</param>
        /// <param name="isOnScene">����� �� ���������������� ��������� �� �����</param>
        /// <returns>������������� ����</returns>
        public static object ObjectField<T>(string name, T gameObject, bool isOnScene) where T : UnityEngine.Object
        {
            return EditorGUILayout.ObjectField(name, gameObject, typeof(T), isOnScene);
        }

        #endregion


        #region bool

        /// <summary>Draws a checkbox in the inspector window. IT IS NECESSARY TO RE-ASSIGN THE VALUE OF THE SWITCH</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="switchedValue">The current value of the switch</param>
        ///<return>New switch value</return>
        public static bool ToogleButton(string content, bool switchedValue)
        {
            return EditorGUILayout.ToggleLeft(content, switchedValue);
        }




        /// <summary>Draws a checkbox in the inspector window. IT IS NECESSARY TO RE-ASSIGN THE VALUE OF THE SWITCH</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="switchedValue">The current value of the switch</param>
        ///<return>New switch value</return>
        public static bool ToogleButton(string content, bool switchedValue, GUILayoutOption[] options)
        {
            return EditorGUILayout.ToggleLeft(content, switchedValue, options);
        }

        #endregion


        #region enum

        /// <summary>Draws a collapsible list with one match</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="switchedValue">The current value of the switch</param>
        ///<return>New switch value</return>
        public static Enum SingleEnumPopup(string fieldName, Enum selectableEnum)
        {
            return EditorGUILayout.EnumPopup(fieldName, selectableEnum);
        }


        /// <summary>Draws a collapsible list with multy match</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="switchedValue">The current value of the switch</param>
        ///<return>New switch value</return>
        public static Enum MultyEnumPopup(string fieldName, Enum selectableEnum)
        {
            return EditorGUILayout.EnumFlagsField(fieldName, selectableEnum);
        }

        #endregion


        #region int


        public static int IntField(int currentValue)
        {
            return EditorGUILayout.IntField(currentValue);
        }


        public static int IntField(string name, int currentValue)
        {
            return EditorGUILayout.IntField(name, currentValue);
        }

        public static int IntField(string name, int currentValue, GUILayoutOption[] options)
        {
            return EditorGUILayout.IntField(name, currentValue, options);
        }

        #endregion


        #region float

        public static float FloatField(float currentValue)
        {
            return EditorGUILayout.FloatField(currentValue);
        }


        public static float FloatField(string name, float currentValue)
        {
            return EditorGUILayout.FloatField(name, currentValue);
        }

        public static float FloatField(string name, float currentValue, GUILayoutOption[] options)
        {
            return EditorGUILayout.FloatField(name, currentValue, options);
        }

        #endregion


        #region string

        /// <summary>
        /// Text input field
        /// </summary>
        /// <param name="text">Field current text</param>
        /// <returns>New field text</returns>
        public static string TextField(string text)
        {
            return EditorGUILayout.TextField(text);
        }


        /// <summary>
        /// Text input field
        /// </summary>
        /// <param name="name">Field display name</param>
        /// <param name="text">Field current text</param>
        /// <returns>New field text</returns>
        public static string TextField(string name, string text)
        {
            return EditorGUILayout.TextField(name, text);
        }

        #endregion


        #region Vector3

        /// <summary>
        /// Text input field
        /// </summary>
        /// <param name="name">Field display name</param>
        /// <param name="value">Field current text</param>
        /// <returns>New field text</returns>
        public static Vector3 Vector3Field(string name, Vector3 value)
        {
            return EditorGUILayout.Vector3Field(name, value);
        }

        #endregion


        #endregion





        #region Slider

        /// <summary>Draws the properties slider in the inspector window</summary>
        /// <param name="value">The property itself</param>
        /// <param name="min">Minimum value of the property</param>
        /// <param name="max">Maximum value of the property</param>
        public static float Slider(float value, float min, float max) => EditorGUILayout.Slider(value, min, max);



        /// <summary>Draws the properties slider in the inspector window</summary>
        /// <param name="value">The property itself</param>
        /// <param name="min">Minimum value of the property</param>
        /// <param name="max">Maximum value of the property</param>
        public static int Slider(int value, int min, int max) => EditorGUILayout.IntSlider(value, min, max);






        /// <summary>Draws the progress in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">The current value is from 0 to 1</param>
        public static void Progressbar(string name, float value)
        {
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), value, name);
        }



        /// <summary>Draws a double slider for two properties in the inspector window</summary>
        /// <param name="currentMinValue">Property for the minimum value</param>
        /// <param name="currentMaxValue">Property for the maximum value</param>
        /// <param name="minLimit">Minimum value of the property</param>
        /// <param name="maxLimit">Maximum value of the property</param>
        public static void DoubleSlider(ref float currentMinValue, ref float currentMaxValue, float minLimit, float maxLimit)
        {
            EditorGUILayout.MinMaxSlider(ref currentMinValue, ref currentMaxValue, minLimit, maxLimit);
        }






        /// <summary>Draws a round progress in the inspector window</summary>
        /// <param name="size">Element size</param>
        /// <param name="currentValue">The current value is from 0 to 1</param>
        /// <param name="minValue">Minimum value of the property</param>
        /// <param name="maxValue">Maximum value of the property</param>
        /// <param name="name">Display name</param>
        /// <param name="background">Color of the blank part</param>
        /// <param name="fill">Color of the filled part</param>
        /// <param name="showValue">Should I show the name and numeric value</param>
        public static void RoundProgressbar(Vector2 size, float currentValue, float minValue, float maxValue, string name, Color background, Color fill, bool showValue)
        {
            EditorGUILayout.Knob(size, currentValue, minValue, maxValue, name, background, fill, showValue);
        }

        #endregion





        #region Groupping

        /// <summary>Starts the horizontal layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        public static void BeginHorizontalGroup(GUIStyle style) => EditorGUILayout.BeginHorizontal(style);

        /// <summary>Starts the horizontal layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        public static void BeginHorizontalGroup(GUIStyle style, GUILayoutOption[] options) => EditorGUILayout.BeginHorizontal(style, options);

        /// <summary>Starts the horizontal layout in the inspector window</summary>
        public static void BeginHorizontalGroup() => EditorGUILayout.BeginHorizontal();

        /// <summary>Closes the horizontal layout in the inspector window</summary>
        public static void EndHorizontalGroup() => EditorGUILayout.EndHorizontal();



        /// <summary>Starts the vertical layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        public static void BeginVerticalGroup(GUIStyle style) => EditorGUILayout.BeginVertical(style);

        /// <summary>Starts the vertical layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        public static void BeginVerticalGroup(GUIStyle style, GUILayoutOption[] options) => EditorGUILayout.BeginVertical(style, options);

        /// <summary>Starts the vertical layout in the inspector window</summary>
        public static void BeginVerticalGroup() => EditorGUILayout.BeginVertical();

        /// <summary>Closes the vertical layout in the inspector window</summary>
        public static void EndVerticalGroup() => EditorGUILayout.EndVertical();





        public static Rect[,] GetRectPosition(int row, int coll, int space = 2)
        {
            return EditorGUILayout.GetControlRect(false, (EditorGUIUtility.singleLineHeight + space) * row).Grid(row, coll, space);
        }


        #endregion





        #region Separators


        /// <summary>Inserts a line spacing (10 pxl) in the inspector window</summary>
        /// <param name="spaceCount">Number of spaces</param>
        public static void Space(int spaceCount = 1)
        {
            for (int i = 0; i < spaceCount; i++)
                EditorGUILayout.Space();
        }


        /// <summary>Inserts a half-line interval (5 pxl) in the inspector window</summary>
        public static void Separator() => EditorGUILayout.Separator();



        /// <summary>Inserts a horizontal line into the inspector window</summary>
        /// <param name="lineHeight">Line thickness</param>
        /// <param name="color">Line color</param>
        public static void Line(int lineHeight = 1, Color? color = null)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);

            rect.height = lineHeight;

            EditorGUI.DrawRect(rect, color == null ? new Color(0.5f, 0.5f, 0.5f, 1) : color.Value);
        }

        #endregion

        #endregion
    }
}
#endif