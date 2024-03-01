using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class ItemCollection : ScriptableObject
{
    [SerializeField] CharacterSkinSetting[] characterSkins;
    [SerializeField]ItemInfo[] items;

    public ItemInfo[] CollectionSnapshot => items;
    public CharacterSkinSetting[] CharacterSkins => characterSkins;

    public ISelectableInfo[] AllItems => characterSkins.Select(s => s as ISelectableInfo)
                                            .Union(items.Select(i => i as ISelectableInfo))
                                            .ToArray();


    public ISelectableInfo[] GetItemsByID(string[] ids)
    {
        return AllItems.Where(i => ids.Contains(i.GetID())).ToArray();
    }

    public ISelectableInfo[] GetItemsByType(SelectableItemType type)
    {
        return AllItems.Where(i => i.GetItemType() == type).ToArray();
    }
}
