using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using System.Linq;

[CommandInfo("Команды", "Выбрать предмет", "Показывает окно выбора предмета")]
public class SelectItemCommand : Command
{
    [Tooltip("Указание пользователю, например \"выберите платье\"")]
    [SerializeField] string message;
    [SerializeField] ItemCollection itemCollection;
    [SerializeField] List<string> startItemsId;
    [SerializeField] List<string> selectedItemsId;
    [SerializeField] SelectItemScreen selectItemScreen;

    public override void OnEnter()
    {
        base.OnEnter();
        StartCoroutine(ProccessRoutine());
    }


    private IEnumerator ProccessRoutine()
    {
        SelectItemScreen selectScreen = selectItemScreen ?? GameManager.Story.UIController.SelectItemScreen;

        selectScreen.Show(GetSelectedItemInfos(), message, GetStartItemInfos());
        yield return new WaitUntil(() => selectScreen.CompleteSelect);
        Continue();
    }


    private ItemInfo[] GetStartItemInfos()
    {
        var allItems = itemCollection.CollectionSnapshot;
        return allItems.Where(i => startItemsId.Contains(i.GetID())).ToArray();
    }

    private ISelectableInfo[] GetSelectedItemInfos()
    {
        if (itemCollection == null)
            return null;

        var allItems = itemCollection.CollectionSnapshot;
        return allItems.Where(i => selectedItemsId.Contains(i.GetID()))
                        .Select(ii => ii as ISelectableInfo)
                        .ToArray();
    }

    public override string GetSummary()
    {
        var items = GetSelectedItemInfos();
        return $"Выбор предметов ({(items.IsNullOrEmpty() ? "" : items[0].GetItemType())})";
    }

    public override Color GetButtonColor()
    {
        return new Color(1f, 0.6f, 0.95f);
    }
}
