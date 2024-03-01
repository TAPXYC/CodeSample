using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;


[CommandInfo("Команды", "Завершить главу", "Показывает окно завершения главы")]
public class CompleteScreenCommand : Command
{
    public override void OnEnter()
    {
        base.OnEnter();

        GameManager.Story.UIController.EndScreen.Show();
    }



    public override string GetSummary()
    {
        return $"Завершение истории";
    }

    public override Color GetButtonColor()
    {
        return new Color(0,7f, 0.4f, 0.9f);
    }
}
