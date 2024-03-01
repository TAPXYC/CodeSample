using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

[CommandInfo("Команды", "Разблокировать предметы", "Добавляет выбранные предметы в инвентарь персонажа")]
public class UnlockAwailableItemsCommand : Command
{
    [SerializeField] ItemCollection itemCollection;
    [SerializeField] List<string> awailableItemsId;

    public override void OnEnter()
    {
        base.OnEnter();
        GameManager.Story.SetAwailableItems(awailableItemsId.ToArray());
        Continue();
    }


    public override string GetSummary()
    {
        return $"Разблокировка предметов {(awailableItemsId.IsNullOrEmpty() ? "" : string.Join(", ", awailableItemsId))}";
    }

    public override Color GetButtonColor()
    {
        return new Color(1f, 0.6f, 0.95f);
    }
}
