using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using System.Linq;

#endif


namespace Tapxyc.Attributes
{

    public class MethodButtonList : PropertyAttribute
    {
        public readonly string header;
        public readonly string[] methods;

        public MethodButtonList(string header, string[] methods)
        {
            this.header = header;
            this.methods = methods;
        }
    }







#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(MethodButtonList))]
    public class MethodButtonListAttributeDrawler : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MethodButtonList data = ((MethodButtonList)attribute);

            var Header = data.header;
            var MethodsName = data.methods;

            var HeadPosition = new Rect(position.position + new Vector2(0, 10), position.size);

            var HeadStyle = new GUIStyle()
            {
                fontSize = 16,
                fontStyle = FontStyle.BoldAndItalic,
                alignment = TextAnchor.UpperCenter
            };
            HeadStyle.normal.textColor = Color.gray;

            GUI.Box(HeadPosition, Header, HeadStyle);



            if (MethodsName.Length != 0)
            {
                var GridPosition = new Rect(position.position + new Vector2(0, 30), position.size - new Vector2(0, 40));
                int selectedindex = GUI.SelectionGrid(GridPosition, -1, MethodsName, MethodsName.Length);

                if (selectedindex != -1)
                {
                    var findMethod = property.serializedObject.targetObject.GetType().GetMethods().FirstOrDefault(m => m.Name == MethodsName[selectedindex]);

                    if (findMethod != null)
                        findMethod.Invoke(property.serializedObject.targetObject, null);
                    else
                        DebugX.ColorMessage($"Method {MethodsName[selectedindex]} not found!", Color.red);
                }
            }
        }




        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 80;
        }
    }

#endif
}