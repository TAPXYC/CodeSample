namespace Tapxyc.Types
{
    using System;
    using UnityEngine;
    using Tapxyc.Drawler;

#if UNITY_EDITOR
    using UnityEditor;
#endif



    [Serializable]
    public class Vector3Interval
    {
        [SerializeField] float minValueX = 0.25f;
        [SerializeField] float maxValueX = 0.75f;
        [SerializeField] float minValueY = 0.25f;
        [SerializeField] float maxValueY = 0.75f;
        [SerializeField] float minValueZ = 0.25f;
        [SerializeField] float maxValueZ = 0.75f;

        [SerializeField] bool canChangeLimit = true;
        [SerializeField] Vector3 minLimit = Vector3.zero;
        [SerializeField] Vector3 maxLimit = Vector3.one;

        public Vector3Interval() { }


        public Vector3Interval(Vector3 minLimit, Vector3 maxLimit, bool canChangeLimit = true)
        {
            this.canChangeLimit = canChangeLimit;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }

        public Vector3 MinValue => new Vector3(minValueX, minValueY, minValueZ);
        public Vector3 MaxValue => new Vector3(maxValueX, maxValueY, maxValueZ);


        public Vector3 RandomValue => new Vector3(Random(minValueX, maxValueX), Random(minValueY, maxValueY), Random(minValueZ, maxValueZ));


        private float Random(float minValue, float maxValue) => UnityEngine.Random.Range(minValue, maxValue);
    }








#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(Vector3Interval))]
    public class Vector3IntervalDrawler : BasePropertyDrawler
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
            SerializedProperty minValueXProp = property.FindPropertyRelative("minValueX");
            SerializedProperty maxValueXProp = property.FindPropertyRelative("maxValueX");
            SerializedProperty minValueYProp = property.FindPropertyRelative("minValueY");
            SerializedProperty maxValueYProp = property.FindPropertyRelative("maxValueY");
            SerializedProperty minValueZProp = property.FindPropertyRelative("minValueZ");
            SerializedProperty maxValueZProp = property.FindPropertyRelative("maxValueZ");
            SerializedProperty minLimitProp = property.FindPropertyRelative("minLimit");
            SerializedProperty maxLimitProp = property.FindPropertyRelative("maxLimit");



            BeginVerticalGroup(EditorStyles.helpBox);

            PrefixLabel(property.displayName, titleStyle);
            isShowLimitValue = ToogleButton("Show Limit Value", isShowLimitValue);

            if (isShowLimitValue)
            {
                GUI.enabled = property.FindPropertyRelative("canChangeLimit").boolValue;

                Property(minLimitProp);
                Property(maxLimitProp);

                GUI.enabled = true;
            }

            ClampProperties(minValueXProp, maxValueXProp, minValueYProp, maxValueYProp, minValueZProp, maxValueZProp, minLimitProp, maxLimitProp);

            Space();
            BeginHorizontalGroup();
            Property(minValueXProp);
            Property(maxValueXProp);
            EndHorizontalGroup();
            DoubleSlider(minValueXProp, maxValueXProp, minLimitProp.vector3Value.x, maxLimitProp.vector3Value.x);
            
            Space();
            BeginHorizontalGroup();
            Property(minValueYProp);
            Property(maxValueYProp);
            EndHorizontalGroup();
            DoubleSlider(minValueYProp, maxValueYProp, minLimitProp.vector3Value.y, maxLimitProp.vector3Value.y);
            
            Space();
            BeginHorizontalGroup();
            Property(minValueZProp);
            Property(maxValueZProp);
            EndHorizontalGroup();
            DoubleSlider(minValueZProp, maxValueZProp, minLimitProp.vector3Value.z, maxLimitProp.vector3Value.z);

            EndVerticalGroup();
        }




        private void ClampProperties(SerializedProperty minValuePropX, SerializedProperty maxValuePropX, SerializedProperty minValuePropY, SerializedProperty maxValuePropY, SerializedProperty minValuePropZ, SerializedProperty maxValuePropZ, SerializedProperty minLimitProp, SerializedProperty maxLimitProp)
        {
            float minValueX = minValuePropX.floatValue;
            float maxValueX = maxValuePropX.floatValue;
            float minValueY  = minValuePropY.floatValue;
            float maxValueY = maxValuePropY.floatValue;
            float minValueZ = minValuePropZ.floatValue;
            float maxValueZ = maxValuePropZ.floatValue;
            Vector3 minLimit = minLimitProp.vector3Value;
            Vector3 maxLimit = maxLimitProp.vector3Value;

            ClampFloat(ref minValueX, ref maxValueX, ref minLimit.x, ref maxLimit.x);
            ClampFloat(ref minValueY, ref maxValueY, ref minLimit.y, ref maxLimit.y);
            ClampFloat(ref minValueZ, ref maxValueZ, ref minLimit.z, ref maxLimit.z);

            minValuePropX.floatValue = minValueX;
            maxValuePropX.floatValue = maxValueX;
            minValuePropY.floatValue = minValueY;
            maxValuePropY.floatValue = maxValueY;
            minValuePropZ.floatValue = minValueZ;
            maxValuePropZ.floatValue = maxValueZ;
            minLimitProp.vector3Value = minLimit;
            maxLimitProp.vector3Value = maxLimit;
        }

        private void ClampFloat(ref float minValue, ref float maxValue, ref float minLimit, ref float maxLimit)
        {
            if (minLimit > maxLimit)
                minLimit = maxLimit;
            if (maxLimit < minLimit)
                maxLimit = minLimit;

            minValue = Mathf.Clamp(minValue, minLimit, maxLimit);
            maxValue = Mathf.Clamp(maxValue, minLimit, maxLimit);

            if (minValue > maxValue)
                minValue = maxValue;

            if (maxValue < minValue)
                maxValue = minValue;
        }
    }

#endif
}