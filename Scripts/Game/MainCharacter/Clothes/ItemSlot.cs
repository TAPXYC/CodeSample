using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Makeup,
    Dress,
    Hair,
}


public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] ItemType type;

    public ItemInfo ItemInfo
    {
        get;
        private set;
    } = ItemInfo.Empty();

    public ItemType SlotType => type;

    private Sprite _emptySprite;

    public void Init(Sprite emptySprite)
    {
        _emptySprite = emptySprite;
    }

    public void SetVisible(bool visible)
    {
        image.sprite = visible ? ItemInfo.Sprite : _emptySprite;
    }

    public void SetItem(ItemInfo item)
    {
        if (item.Type == type)
        {
            ItemInfo = item;
            image.sprite = ItemInfo.Sprite;
            //Debug.LogError($"{type} set {ItemInfo.GetName()}");
        }
        else
        {
            Debug.LogError($"НЕПОДХОДЯЩИЙ ТИП - {item.Type} (нужно {type})");
        }
    }
}
