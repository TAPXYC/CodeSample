#if UNITY_EDITOR
namespace Tapxyc.Drawler
{
    using UnityEngine;
    using UnityEditor;
    using TapxycRectEx;

    public abstract class BasePropertyDrawler : PropertyDrawer
    {
        protected GUIStyle subTittle;
        protected GUIStyle errorStyle;
        protected Color? impotantFieldWrongColor = Color.red;
        protected SerializedObject serializedObject;
        protected MonoBehaviour owner;

        private bool _isInitialized = false;



        public BasePropertyDrawler()
        {
            errorStyle = new GUIStyle()
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            errorStyle.normal.textColor = new Color(1, 0.3f, 0.3f);

            impotantFieldWrongColor = new Color(1, 0.35f, 0.35f);


            subTittle = new GUIStyle()
            {
                fontSize = 12,
                fontStyle = FontStyle.BoldAndItalic,
                alignment = TextAnchor.MiddleLeft
            };
            subTittle.normal.textColor = Color.white;

            OnCreate();
        }


        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }


        public sealed override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                serializedObject = property.serializedObject;
                owner = serializedObject.targetObject as MonoBehaviour;
                
                OnInit(property);
            }

            OnDraw(rect, property, label);
        }



        #region Draw elements methods

        #region Label

        /// <summary>Draws a simple label in the inspector window</summary>
        /// <param name="text">Displayed text</param>
        protected void PrefixLabel(string text) => StaticDrawler.PrefixLabel(text);



        /// <summary>Draws a simple label in the inspector window</summary>
        /// <param name="text">Displayed text</param>
        /// <param name="labelStyle">Text style</param>
        protected void PrefixLabel(string content, GUIStyle labelStyle) => StaticDrawler.PrefixLabel(content, labelStyle);




        /// <summary>Draws a simple label in the inspector window</summary>
        /// <param name="text">Displayed text</param>
        /// <param name="labelStyle">Text style</param>
        protected void PrefixLabel(string content, Rect position, GUIStyle labelStyle) => StaticDrawler.PrefixLabel(content, position, labelStyle);




        /// <summary>Draws a simple label in the inspector window that can be copied</summary>
        /// <param name="text">Displayed text</param>
        protected void CopyEnabledLabel(string text) => StaticDrawler.CopyEnabledLabel(text);


        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        protected void Label(string value) => StaticDrawler.Label(value);


        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        protected void Label(string value, GUIStyle style) => StaticDrawler.Label(value, style);



        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        protected void Label(string name, string value) => StaticDrawler.Label(name, value);


        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>
        protected void Label(string name, string value, GUIStyle style) => StaticDrawler.Label(name, value, style);




        /// <summary>Draws a field by property type (name on the left, value on the right) in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">Displayed value</param>    
        protected void Label(string name, Texture value) => StaticDrawler.Label(name, value);

        #endregion





        #region Switcher

        /// <summary>Draws a button in the inspector window</summary>
        /// <param name="content">The inscription on the button</param>
        ///<return>Whether the button was pressed or not</return>
        protected bool Button(string content) => StaticDrawler.Button(content);



        /// <summary>Draws a button in the inspector window</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="buttonStyle">Button Style</param>
        ///<return>Whether the button was pressed or not</return>
        protected bool Button(string content, GUIStyle buttonStyle) => StaticDrawler.Button(content, buttonStyle);





        /// <summary>Draws a checkbox in the inspector window. IT IS NECESSARY TO RE-ASSIGN THE VALUE OF THE SWITCH</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="switchedValue">The current value of the switch</param>
        ///<return>New switch value</return>
        protected bool ToogleButton(string content, bool switchedValue) => StaticDrawler.ToogleButton(content, switchedValue);






        /// <summary>Draws a checkbox in the inspector window. IT IS NECESSARY TO RE-ASSIGN THE VALUE OF THE SWITCH</summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="switchedValue">The current value of the switch</param>
        ///<return>New switch value</return>
        protected bool ToogleButton(string content, bool switchedValue, GUILayoutOption[] options) => StaticDrawler.ToogleButton(content, switchedValue, options);




        /// <summary>Draws a collapsible list </summary>
        /// <param name="content">The inscription on the button</param>
        /// <param name="switchedValue">The current value of the switch</param>
        ///<return>New switch value</return>
        protected System.Enum EnumPopup(string fieldName, System.Enum selectableEnum) => StaticDrawler.SingleEnumPopup(fieldName, selectableEnum);

        #endregion





        #region Property

        /// <summary>Draws the property in the inspector window</summary>
        /// <param name="mainProperty">Object Property</param>
        /// <param name="propName">Name of the desired property property</param>
        /// <param name="displayName">Display name</param>
        /// <param name="tooltip">Pop-up description</param>
        /// <param name="isImportant">Whether this property is important. If yes, then if this field is not filled in, it will be highlighted with the error color and will be signed as an error</param>
        protected void PropertyFromMain(SerializedProperty mainProperty, string propName, string displayName = "", string tooltip = "", bool isImportant = false)
        {
            Property(mainProperty.FindPropertyRelative(propName), displayName, tooltip, isImportant);
        }



        /// <summary>Draws the property in the inspector window</summary>
        /// <param name="property">The property itself</param>
        /// <param name="displayName">Display name</param>
        /// <param name="tooltip">Pop-up description</param>
        /// <param name="isImportant">Whether this property is important. If yes, then if this field is not filled in, it will be highlighted with the error color and will be signed as an error</param>
        protected void Property(SerializedProperty property, string displayName = "none", string tooltip = "", bool isImportant = false)
        {
            Color prevBackground = GUI.color;

            var content = new GUIContent(displayName, tooltip);
            content.text = displayName == "none" ? property.displayName : displayName;

            if (isImportant)
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                    if (property.objectReferenceValue == null)
                    {
                        PrefixLabel("ERROR! Required field", errorStyle);
                        GUI.color = impotantFieldWrongColor.Value;
                    }
            }

            EditorGUILayout.PropertyField(property, content);

            GUI.color = prevBackground;
        }






        /// <summary>Draws the property in the inspector window</summary>
        /// <param name="property">The property itself</param>
        /// <param name="displayName">Display name</param>
        /// <param name="tooltip">Pop-up description</param>
        /// <param name="isImportant">Whether this property is important. If yes, then if this field is not filled in, it will be highlighted with the error color and will be signed as an error</param>
        protected void Property(SerializedProperty property, Rect position, string displayName = "", string tooltip = "", bool isImportant = false)
        {
            Color prevBackground = GUI.color;

            var content = new GUIContent(displayName, tooltip);
            content.text = displayName == "" ? property.displayName : displayName;

            var contentPosition = position;

            if (isImportant)
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                    if (property.objectReferenceValue == null)
                    {
                        var splitPositions = position.Grid(2, 1);
                        contentPosition = splitPositions[1, 0];

                        PrefixLabel("ERROR! Required field", splitPositions[0, 0], errorStyle);
                        GUI.color = impotantFieldWrongColor.Value;
                    }
            }

            EditorGUI.PropertyField(contentPosition, property, content);

            GUI.color = prevBackground;
        }






        protected int IntField(string name, int currentValue) => StaticDrawler.IntField(name, currentValue);

        protected int IntField(string name, int currentValue, GUILayoutOption[] options) => StaticDrawler.IntField(name, currentValue, options);

        #endregion





        #region Slider

        /// <summary>Draws the properties slider in the inspector window</summary>
        /// <param name="property">The property itself</param>
        /// <param name="min">Minimum value of the property</param>
        /// <param name="max">Maximum value of the property</param>
        protected void Slider(SerializedProperty property, float min, float max) => EditorGUILayout.Slider(property, min, max);




        /// <summary>Draws a double slider for two properties in the inspector window</summary>
        /// <param name="minProp">Property for the minimum value</param>
        /// <param name="maxProp">Property for the maximum value</param>
        /// <param name="minLimit">Minimum value of the property</param>
        /// <param name="maxLimit">Maximum value of the property</param>
        protected void DoubleSlider(SerializedProperty minProp, SerializedProperty maxProp, float minLimit, float maxLimit)
        {
            float min = minProp.floatValue;
            float max = maxProp.floatValue;

            StaticDrawler.DoubleSlider(ref min, ref max, minLimit, maxLimit);

            minProp.floatValue = min;
            maxProp.floatValue = max;
        }





        /// <summary>Draws the progress in the inspector window</summary>
        /// <param name="name">Display name</param>
        /// <param name="value">The current value is from 0 to 1</param>
        protected void Progressbar(string name, float value) => StaticDrawler.Progressbar(name, value);




        /// <summary>Draws a round progress in the inspector window</summary>
        /// <param name="size">Element size</param>
        /// <param name="currentValue">The current value is from 0 to 1</param>
        /// <param name="minValue">Minimum value of the property</param>
        /// <param name="maxValue">Maximum value of the property</param>
        /// <param name="name">Display name</param>
        /// <param name="background">Color of the blank part</param>
        /// <param name="fill">Color of the filled part</param>
        /// <param name="showValue">Should I show the name and numeric value</param>
        protected void RoundProgressbar(Vector2 size, float currentValue, float minValue, float maxValue, string name, Color background, Color fill, bool showValue) =>
            StaticDrawler.RoundProgressbar(size, currentValue, minValue, maxValue, name, background, fill, showValue);

        #endregion





        #region Groupping

        /// <summary>Starts the horizontal layout in the inspector window</summary>
        protected void BeginHorizontalGroup() => StaticDrawler.BeginHorizontalGroup();

        /// <summary>Starts the horizontal layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        protected void BeginHorizontalGroup(GUIStyle style) => StaticDrawler.BeginHorizontalGroup(style);

        /// <summary>Starts the horizontal layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        protected void BeginHorizontalGroup(GUIStyle style, GUILayoutOption[] options) => StaticDrawler.BeginHorizontalGroup(style, options);

        /// <summary>Closes the horizontal layout in the inspector window</summary>
        protected void EndHorizontalGroup() => StaticDrawler.EndHorizontalGroup();




        /// <summary>Starts the vertical layout in the inspector window</summary>
        protected void BeginVerticalGroup() => StaticDrawler.BeginVerticalGroup();

        /// <summary>Starts the vertical layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        protected void BeginVerticalGroup(GUIStyle style) => StaticDrawler.BeginVerticalGroup(style);

        /// <summary>Starts the vertical layout in the inspector window</summary>
        /// <param name="style">Border Style</param>
        protected void BeginVerticalGroup(GUIStyle style, GUILayoutOption[] options) => StaticDrawler.BeginVerticalGroup(style, options);

        /// <summary>Closes the vertical layout in the inspector window</summary>
        protected void EndVerticalGroup() => StaticDrawler.EndVerticalGroup();





        protected Rect[,] GetRectPosition(int row, int coll, int space = 2) => StaticDrawler.GetRectPosition(row, coll, space);


        #endregion





        #region Separators


        /// <summary>Inserts a line spacing (10 pxl) in the inspector window</summary>
        /// <param name="spaceCount">Number of spaces</param>
        protected void Space(int spaceCount = 1) => StaticDrawler.Space(spaceCount);


        /// <summary>Inserts a half-line interval (5 pxl) in the inspector window</summary>
        protected void Separator() => StaticDrawler.Separator();



        /// <summary>Inserts a horizontal line into the inspector window</summary>
        /// <param name="lineHeight">Line thickness</param>
        /// <param name="color">Line color</param>
        protected void Line(int lineHeight = 1, Color? color = null) => StaticDrawler.Line(lineHeight, color);

        #endregion

        #endregion




        #region Logic

        protected SerializedProperty ClampMinAndReturn(SerializedProperty mainProperty, string propertyName, float clampMinValue)
        {
            var Prop = mainProperty.FindPropertyRelative(propertyName);
            Prop.ClampMin(clampMinValue);

            return Prop;
        }

        #endregion


        #region Abstract

        protected abstract void OnCreate();
        protected abstract void OnInit(SerializedProperty property);
        protected abstract void OnDraw(Rect rect, SerializedProperty property, GUIContent label);

        #endregion
    }

}
#endif