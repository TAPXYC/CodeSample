using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;


[CommandInfo("Команды", "Конец загрузки", "Завершает процесс загрузки истории")]
public class StoryLoadedCommand : Command
{
    public override void OnEnter()
    {
        if (GameManager.Story.IsLoadedFromData)
            StopParentBlock();
        else
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
