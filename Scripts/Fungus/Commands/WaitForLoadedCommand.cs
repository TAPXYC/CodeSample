using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

[CommandInfo("Команды", "Начало загрузки", "Ждет загрузки данных истории")]
public class WaitForLoadedCommand : Command
{
    public override void OnEnter()
    {
        StartCoroutine(WaitCoroutine());
    }


    private IEnumerator WaitCoroutine()
    {
        yield return new WaitUntil(() => StoryController.IsLoaded);
        Continue();
    }

    public override string GetSummary()
    {
        return $"Завершение инициализации истории";
    }

    public override Color GetButtonColor()
    {
        return new Color(0, 1, 1);
    }
}