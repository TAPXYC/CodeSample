namespace Tapxyc.Types
{
    using System;
    using UnityEngine;
    using Tapxyc.Drawler;

#if UNITY_EDITOR
    using UnityEditor;
#endif



    [Serializable]
    public class IntInterval
    {
        [SerializeField] int minValue = 0;
        [SerializeField] int maxValue = 1;

        [SerializeField] bool canChangeLimit = true;
        [SerializeField] int minLimit = 0;
        [SerializeField] int maxLimit = 1;



        public float MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = (int)Mathf.Clamp(value, minLimit, maxLimit);
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
                minValue = (int)Mathf.Clamp(value, minLimit, maxLimit);
            }
        }




        public int RandomValue => UnityEngine.Random.Range(minValue, maxValue);


        public IntInterval() { }


        public IntInterval(int minLimit, int maxLimit, bool canChangeLimit = true)
        {
            this.canChangeLimit = canChangeLimit;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }


        public int Proportion(float coef) => minValue + (int)((maxValue - minValue) * coef);
    }








#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(IntInterval))]
    public class IntIntervalDrawler : BasePropertyDrawler
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

            minLimitProp.ClampMax(maxLimitProp, 0);
            maxLimitProp.ClampMin(minLimitProp, 0);

            minValueProp.ClampMin(minLimitProp, 0);
            minValueProp.ClampMax(maxLimitProp, 0);

            maxValueProp.ClampMin(minLimitProp, 0);
            maxValueProp.ClampMax(maxLimitProp, 0);

            minValueProp.ClampMax(maxValueProp, 0);
            maxValueProp.ClampMin(minValueProp, 0);


            Property(minValueProp);
            Property(maxValueProp);

            float min = minValueProp.intValue;
            float max = maxValueProp.intValue;

            StaticDrawler.DoubleSlider(ref min, ref max, minLimitProp.intValue, maxLimitProp.intValue);

            minValueProp.intValue = (int)min;
            maxValueProp.intValue = (int)max;

            EndVerticalGroup();
        }
    }

#endif
}