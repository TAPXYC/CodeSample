namespace Tapxyc.Types
{
    using System;
    using UnityEngine;
    using Tapxyc.Drawler;

#if UNITY_EDITOR
    using UnityEditor;
#endif



    [Serializable]
    public class Interval
    {
        [SerializeField] float minValue = 0.25f;
        [SerializeField] float maxValue = 0.75f;

        [SerializeField] bool canChangeLimit = true;
        [SerializeField] float minLimit = 0;
        [SerializeField] float maxLimit = 1;


        public float MaxLimit => maxLimit;
        public float MinLimit => minLimit;

        public float MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = Mathf.Clamp(value, minLimit, maxLimit);
            }
        }



        public float MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                minValue = Mathf.Clamp(value, minLimit, maxLimit);
            }
        }


        public float RandomValue => UnityEngine.Random.Range(minValue, maxValue);


        public Interval() { }
        

        public Interval(float minLimit, float maxLimit, bool canChangeLimit = true)
        {
            this.canChangeLimit = canChangeLimit;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }



        public float ClampValue(float value) => Mathf.Clamp(value, minValue, maxValue);


        public float Proportion(float coef) => minValue + (maxValue - minValue) * coef;

        public float PercentFromValue(float value) => value.PercentFromSize(minValue, maxValue);
    }








#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(Interval))]
    public class IntervalDrawler : BasePropertyDrawler
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

            minLimitProp.ClampMax(maxLimitProp, 0f);
            maxLimitProp.ClampMin(minLimitProp, 0f);

            minValueProp.ClampMin(minLimitProp, 0f);
            minValueProp.ClampMax(maxLimitProp, 0f);

            maxValueProp.ClampMin(minLimitProp, 0f);
            maxValueProp.ClampMax(maxLimitProp, 0f);

            minValueProp.ClampMax(maxValueProp, 0f);
            maxValueProp.ClampMin(minValueProp, 0f);


            Property(minValueProp);
            Property(maxValueProp);

            DoubleSlider(minValueProp, maxValueProp, minLimitProp.floatValue, maxLimitProp.floatValue);

            EndVerticalGroup();
        }
    }

#endif
}