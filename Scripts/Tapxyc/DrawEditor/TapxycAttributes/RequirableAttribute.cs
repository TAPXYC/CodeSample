#if UNITY_EDITOR
using UnityEditor;
using Tapxyc.Drawler;
#endif

using UnityEngine;

public class RequirableAttribute : PropertyAttribute
{
    public RequirableAttribute()
    {

    }
}



#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(RequirableAttribute))]
public class RequirableAttributeDrawler : BasePropertyDrawler
{
    protected override void OnCreate() { }
    protected override void OnInit(SerializedProperty property) { }

    

    protected override void OnDraw(Rect rect, SerializedProperty property, GUIContent label)
    {
        Property(property, isImportant: true);
    }
}

#endif

