using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

[CommandInfo("Команды", "Ввести текст", "Показывает окно ввода текста и сохраняет текст в указанную переменную")]
public class InputNameCommand : Command
{
    [SerializeField] InputNameWindow inputNameWindow;
    [Tooltip("Указание пользователю, например \"введите имя\"")]
    [SerializeField] string placeholder;
    [SerializeField] StringData textVariable;

    public override void OnEnter()
    {
        base.OnEnter();
        StartCoroutine(ProccessRoutine());
    }


    private IEnumerator ProccessRoutine()
    {
        InputNameWindow input = inputNameWindow ?? GameManager.Story.UIController.InputNameWindow;
        yield return input.Show(placeholder);
        textVariable.Value = input.InputedText;
        Continue();
    }


    public override string GetSummary()
    {
        return $"Ввод текста ({placeholder})";
    }

    public override Color GetButtonColor()
    {
        return new Color(0, 1, 1);
    }
}
