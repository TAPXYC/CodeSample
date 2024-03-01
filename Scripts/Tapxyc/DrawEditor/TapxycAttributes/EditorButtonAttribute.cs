using System;

#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#endif


namespace Tapxyc.Attributes
{
    /// <summary>
    /// Attribute on methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EditorButtonAttribute : Attribute
    {
        /// <summary>
        /// Button text
        /// </summary>
        public string name;

        /// <summary>
        /// Adding a button to the Inspector
        /// </summary>
        /// <param name="name">Button text</param>
        public EditorButtonAttribute(string name)
        {
            this.name = name;
        }
    }










#if UNITY_EDITOR

    /// <summary>
    /// Drawing a button in the inspector
    /// </summary>
    [CustomEditor(typeof(UnityEngine.Object), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class ButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var target in targets)
            {
                var mis = target.GetType().GetMethods().Where(m => m.GetCustomAttributes().Any(a => a.GetType() == typeof(EditorButtonAttribute)));
                if (mis != null)
                {
                    foreach (var mi in mis)
                    {
                        if (mi != null)
                        {
                            var attribute = (EditorButtonAttribute)mi.GetCustomAttribute(typeof(EditorButtonAttribute));

                            if (GUILayout.Button(attribute.name))
                            {
                                mi.Invoke(target, null);
                            }
                        }
                    }
                }
            }
        }
    }

    #endif
}