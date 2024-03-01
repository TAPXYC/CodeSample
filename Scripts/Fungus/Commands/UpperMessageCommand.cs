using Fungus;
using UnityEngine;

[CommandInfo("Narrative", "Say upper","Показывает сообщение сверху")]
public class UpperMessageCommand : Command
{
    [TextArea(5, 10)]
    [SerializeField] string message;
    [Tooltip("Таймер для автоматического переключения через заданное количество секунд. [0 - 0.5] - продолжение по клику")]
    [SerializeField] float showDuration = 0;
    [Tooltip("Кастомный диалог (по умолчанию задан на канвасе)")]
    [SerializeField] SimpleMessageDialog setSayDialog;
    [Tooltip("Voiceover audio to play when writing the text")]
    [SerializeField] protected AudioClip voiceOverClip;

    public override void OnEnter()
    {
        SimpleMessageDialog dialog = setSayDialog ?? GameManager.Story.UIController.UpperDialog;

        if (dialog == null)
        {
            Debug.LogError("Не задан верхний диалог!");
            Continue();
            return;
        }

        if (showDuration > 0.5f)
            dialog.Show(message, showDuration, () => Continue(), voiceOverClip);
        else
            dialog.Show(message, () => Continue(), voiceOverClip);
    }

    public override string GetSummary()
    {
        return $"{message}";
    }

    public override Color GetButtonColor()
    {
        return new Color(0.6f, 1, 0.2f);
    }
}
