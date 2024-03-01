using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using System.Linq;

[CommandInfo("Команды", "Создать ГГ", "Показывает окно выбора внешности")]
public class InitPersonCommand : Command
{
    [Tooltip("Указание пользователю, например \"выберите платье\"")]
    [SerializeField] string message;
    [SerializeField] ItemCollection itemCollection;
    [SerializeField] List<string> startItemsId;
    [SerializeField] SelectItemScreen selectItemScreen;

    public override void OnEnter()
    {
        base.OnEnter();
        StartCoroutine(ProccessRoutine());
    }


    private IEnumerator ProccessRoutine()
    {
        SelectItemScreen selectScreen = selectItemScreen ?? GameManager.Story.UIController.SelectItemScreen;

        GameManager.Story.StoryStage.MainCharacter.SetDefaultItems(GetDefaultItemInfos());

        selectScreen.Show(GetSelectedItemInfos(), message);
        yield return new WaitUntil(() => selectScreen.CompleteSelect);
        Continue();
    }


    private ItemInfo[] GetDefaultItemInfos()
    {
        var allItems = itemCollection.CollectionSnapshot;
        return allItems.Where(i => startItemsId.Contains(i.GetID())).ToArray();
    }

    private ISelectableInfo[] GetSelectedItemInfos()
    {
        return GameManager.Story.Items.CharacterSkins;
    }

    public override string GetSummary()
    {
        return $"Выбор внешности ГГ";
    }

    public override Color GetButtonColor()
    {
        return new Color(1f, 0.6f, 0.95f);
    }
}
