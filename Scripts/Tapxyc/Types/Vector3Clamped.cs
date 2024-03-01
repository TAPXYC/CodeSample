namespace Tapxyc.Types
{
    using System;
    using UnityEngine;
    using Tapxyc.Drawler;

#if UNITY_EDITOR
    using UnityEditor;
#endif



    [Serializable]
    public class Vector3Clamped
    {
        [SerializeField] Vector3 minValue = Vector3.zero;
        [SerializeField] Vector3 maxValue = Vector3.one;

        [SerializeField] bool canChangeLimit = true;
        [SerializeField] float valueX = 0.5f;
        [SerializeField] float valueY = 0.5f;
        [SerializeField] float valueZ = 0.5f;

        public float X => valueX;
        public float Y => valueY;
        public float Z => valueZ;


        public Vector3 GetRandomVector() => new Vector3(UnityEngine.Random.Range(minValue.x, maxValue.x), 
                                                        UnityEngine.Random.Range(minValue.y, maxValue.y), 
                                                        UnityEngine.Random.Range(minValue.z, maxValue.z));

        public Vector3Clamped() { }


        public Vector3Clamped(Vector3 minValue, Vector3 maxValue, bool canChangeLimit = true)
        {
            this.canChangeLimit = canChangeLimit;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }








#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(Vector3Clamped))]
    public class Vector3ClampedDrawler : BasePropertyDrawler
    {
        private bool isShowLimitValue;
        GUIStyle titleStyle;


        protected override void OnCreate()
        {
            titleStyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };
            titleStyle.normal.textColor = Color.white;
        }



        protected override void OnInit(SerializedProperty property) { }



        protected override void OnDraw(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty minValueProp = property.FindPropertyRelative("minValue");
            SerializedProperty maxValueProp = property.FindPropertyRelative("maxValue");
            SerializedProperty valueXProp = property.FindPropertyRelative("valueX");
            SerializedProperty valueYProp = property.FindPropertyRelative("valueY");
            SerializedProperty valueZProp = property.FindPropertyRelative("valueZ");


            BeginVerticalGroup(EditorStyles.helpBox);

            PrefixLabel(property.displayName, titleStyle);
            isShowLimitValue = ToogleButton("Show Limit Value", isShowLimitValue);

            if (isShowLimitValue)
            {
                GUI.enabled = property.FindPropertyRelative("canChangeLimit").boolValue;

                Property(minValueProp);
                Property(maxValueProp);

                GUI.enabled = true;
            }

            ClampProperties(minValueProp, maxValueProp, valueXProp, valueYProp, valueZProp);

            Space();
            Slider(valueXProp, minValueProp.vector3Value.x, maxValueProp.vector3Value.x);
            Slider(valueYProp, minValueProp.vector3Value.y, maxValueProp.vector3Value.y);
            Slider(valueZProp, minValueProp.vector3Value.z, maxValueProp.vector3Value.z);

            EndVerticalGroup();
        }




        private void ClampProperties(SerializedProperty minValueProp, SerializedProperty maxValueProp, SerializedProperty valueXProp, SerializedProperty valueYProp, SerializedProperty valueZProp)
        {
            Vector3 minValue = minValueProp.vector3Value;
            Vector3 maxValue = maxValueProp.vector3Value;
            float valueX = valueXProp.floatValue;
            float valueY = valueYProp.floatValue;
            float valueZ = valueZProp.floatValue;

            ClampFloat(ref minValue.x, ref maxValue.x, ref valueX);
            ClampFloat(ref minValue.y, ref maxValue.y, ref valueY);
            ClampFloat(ref minValue.z, ref maxValue.z, ref valueZ);

            minValueProp.vector3Value = minValue;
            maxValueProp.vector3Value = maxValue;
            valueXProp.floatValue = valueX;
            valueYProp.floatValue = valueY;
            valueZProp.floatValue = valueZ;
        }

        private void ClampFloat(ref float minValue, ref float maxValue, ref float value)
        {
            if(minValue > maxValue)
                minValue = maxValue;

            if(maxValue < minValue)
                maxValue = minValue;

            value = Mathf.Clamp(value, minValue, maxValue);
        }
    }

#endif
}