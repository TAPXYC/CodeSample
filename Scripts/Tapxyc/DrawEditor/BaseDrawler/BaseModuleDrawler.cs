#if UNITY_EDITOR
namespace Tapxyc.Drawler
{
    using UnityEditor;
    using UnityEngine;


    public abstract class BaseModuleDrawler : BasePropertyDrawler
    {
        protected bool Show
        {
            get;
            private set;
        } = true;


        protected GUIStyle headerStyle;


        private string title;
        private GUIStyle backgroundStyle;
        private Color titleColor;
        


        #region Override

        protected override void OnCreate()
        {
            int titleFontSize;
            FontStyle titleFontStyle;

            OnCreate(out title, out titleFontSize, out titleColor, out titleFontStyle, out backgroundStyle, out impotantFieldWrongColor);

            backgroundStyle = backgroundStyle == null ?
                                    EditorStyles.helpBox :
                                    backgroundStyle;

            impotantFieldWrongColor = impotantFieldWrongColor == null ?
                                    new Color(1, 0.35f, 0.35f) :
                                    impotantFieldWrongColor;

            headerStyle = new GUIStyle()
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = titleFontSize,
                fontStyle = titleFontStyle
            };
            headerStyle.normal.textColor = titleColor;
        }







        protected override void OnDraw(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginVertical(backgroundStyle);

            DrawHead(rect, property);

            if (Show)
            {
                Line(2, titleColor);
                Separator();

                DrawBody(rect, property);
            }

            EditorGUILayout.EndVertical();
        }


        protected override void OnInit(SerializedProperty property) 
        { 
            OnInitDrawler(property);
        }


        #endregion


        #region Control draw overrides methods


        private void DrawHead(Rect rect, SerializedProperty property)
        {
            if (Button($"{property.displayName}  ({title})", headerStyle))
                Show = !Show;

            GUI.Box(new Rect(rect.position + new Vector2(3, 4), Vector2.one * 24), Show ? @"v" : ">");

            Space();
        }






        private void DrawBody(Rect rect, SerializedProperty property)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Separator();

            OnDrawBody(rect, property);

            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;
        }

        #endregion




        #region  Abstract methods

        protected abstract void OnCreate(out string title, out int titleFontSize, out Color titleColor, out FontStyle titleFontStyle, out GUIStyle backgroundStyle, out Color? impotantFieldColor);
        protected abstract void OnDrawBody(Rect rect, SerializedProperty property);
        protected abstract void OnInitDrawler(SerializedProperty property);
        #endregion
    }
}
#endif