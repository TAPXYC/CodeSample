using Fungus;
using UnityEngine;



[CommandInfo("Команды", "Изменить параметр", "Изменение значения переменной параметра и вывести сообщение об изменении")]
public class ChangeVariablesCommand : Command
{
    [SerializeField] string parameterName;
    [SerializeField] IntegerData variable;
    [SerializeField] int addValue;
    [Space]
    [Tooltip("Таймер для автоматического переключения через заданное количество секунд. [0 - 0.5] - продолжение по клику")]
    [SerializeField] float showDuration = 0;
    [Tooltip("Кастомный диалог (по умолчанию задан на канвасе)")]
    [SerializeField] SimpleMessageDialog setSayDialog;

    [Tooltip("Voiceover audio to play when writing the text")]
    [SerializeField] protected AudioClip voiceOverClip;

    public override void OnEnter()
    {
        variable.Value += addValue;

        SimpleMessageDialog dialog = setSayDialog ?? GameManager.Story.UIController.ParameterDialog;

        if (dialog == null)
        {
            Debug.LogError("Не задан верхний диалог!");
            Continue();
            return;
        }

        string text = $"{parameterName} + {addValue}";

        if (showDuration > 0.5f)
            dialog.Show(text, showDuration, () => Continue(), voiceOverClip);
        else
            dialog.Show(text, () => Continue(), voiceOverClip);


    }

    public override string GetSummary()
    {
        return $"{variable.GetDescription()} + {addValue}";
    }

    public override Color GetButtonColor()
    {
        return new Color(1, 1, 1);
    }
}
