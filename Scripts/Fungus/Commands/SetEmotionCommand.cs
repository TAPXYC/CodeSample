using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Fungus.EditorUtils;
#endif

[CommandInfo("Narrative", "Portrait Emotion", "Меняет эмоцию ГГ")]
public class SetEmotionCommand : Command
{
    [SerializeField] EmotionType emotion;
    [SerializeField] string emotionId;

    public override void OnEnter()
    {
        base.OnEnter();
        var mainCharacter = GameManager.Story.StoryStage.MainCharacter;

        if (emotion == EmotionType.Custom)
            mainCharacter.SetEmotion(emotionId);
        else
            mainCharacter.SetEmotion(emotion);

        Continue();
    }


    public override string GetSummary()
    {
        return $"Эмоция {(emotion == EmotionType.Custom ? emotionId : emotion)}";
    }

    public override Color GetButtonColor()
    {
        return new Color(0, 1, 1);
    }
}



#if UNITY_EDITOR

[CustomEditor(typeof(SetEmotionCommand))]
public class SetEmotionCommandEditor : CommandEditor
{
    protected SerializedProperty emotionProp;
    protected SerializedProperty emotionIdProp;

    public override void OnEnable()
    {
        base.OnEnable();
        emotionProp = serializedObject.FindProperty("emotion");
        emotionIdProp = serializedObject.FindProperty("emotionId");
    }

    public override void DrawCommandGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(emotionProp);

        if ((EmotionType)(emotionProp.enumValueIndex) == EmotionType.Custom)
            EditorGUILayout.PropertyField(emotionIdProp);

        serializedObject.ApplyModifiedProperties();
    }
}

#endif