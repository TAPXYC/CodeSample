using Fungus;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Coffee.UISoftMask;

public class MainCharacterController : MonoBehaviour
{
    [SerializeField] Sprite emptySprite;
    [Space]
    [SerializeField] SoftMask mainMask;
    [SerializeField] Image portraitImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] ItemSlot[] itemSlots;
    [Space]
    [SerializeField] CharacterSkin skinController;

    public ISelectableInfo[] GetAllSelectedItems() => GetSelectedItems().Select(i => i as ISelectableInfo)
                                                                    .Append(skinController.Skin)
                                                                    .ToArray();
    public ItemInfo[] GetSelectedItems() => itemSlots.Select(i => i.ItemInfo)
                                                        .Where(i => i.GetIsCreated())
                                                        .ToArray();
    public CharacterSkinSetting Skin => skinController.Skin;

    public Stat[] Stats
    {
        get;
        private set;
    }

    public StringData CharacterName
    {
        get;
        private set;
    }
    public Character Character => _characterSettings;

    private Character _characterSettings;
    private Tween _fadeTween;



    public void Init(Character characterSettings, ISelectableInfo[] selectedItems)
    {
        _characterSettings = characterSettings;

        _characterSettings.State.holder = GetComponent<RectTransform>();
        _characterSettings.State.portraitImage = portraitImage;
        _characterSettings.State.allPortraits.Add(portraitImage);

        foreach (var item in selectedItems)
        {
            if (item is CharacterSkinSetting)
                SetSkin(item as CharacterSkinSetting);

            if (item is ItemInfo)
                SetItem((item as ItemInfo?).Value);
        }

        Init();
    }


    public void Init()
    {
        skinController.Init();

        foreach (var item in itemSlots)
        {
            item.Init(emptySprite);
        }
    }



    public void SetSkin(CharacterSkinSetting skin)
    {
        backgroundImage.sprite = skinController.SetSkin(skin);
        SetItemsVisible(!skinController.HideItems, !skinController.HideHair);
        //Debug.LogError($"скин {skin.name}: items - {!skinController.HideItems}, hair - {!skinController.HideHair}");
    }


    public void SetEmotion(EmotionType emotion)
    {
        backgroundImage.sprite = skinController.SetEmotion(emotion);
        SetItemsVisible(!skinController.HideItems, !skinController.HideHair);

        //Debug.LogError($"Эмоция {emotion}: items - {!skinController.HideItems}, hair - {!skinController.HideHair}");
    }

    public void SetEmotion(string emotionID)
    {
        backgroundImage.sprite = skinController.SetEmotion(emotionID);
        SetItemsVisible(!skinController.HideItems, !skinController.HideHair);
        //Debug.LogError($"Эмоция {emotionID}: items - {!skinController.HideItems}, hair - {!skinController.HideHair}");
    }


    public void SetStats(Stat[] stats, StringData characterName)
    {
        Stats = stats;
        CharacterName = characterName;
    }



    public bool HasItemOfType(ItemType type)
    {
        return GetSelectedItems().Any(i => i.Type == type);
    }

    public void SetItems(ItemInfo[] items)
    {
        if (!items.IsNullOrEmpty())
        {
            foreach (var item in items)
            {
                SetItem(item);
            }
        }
    }


    public void SetItems(ISelectableInfo[] items)
    {
        if (!items.IsNullOrEmpty())
        {
            foreach (var item in items)
            {
                if (item is ItemInfo)
                    SetItem((item as ItemInfo?).Value);

                if (item is CharacterSkinSetting)
                    SetSkin(item as CharacterSkinSetting);
            }
        }
    }

    public void SetDefaultItems(ItemInfo[] items)
    {
        if (!items.IsNullOrEmpty())
        {
            //Debug.LogError($"SET DEFAULT {string.Join(", ", items.Select(i => i.GetName() + " " + i.Type))}");
            foreach (var item in items)
            {
                if (!HasItemOfType(item.Type))
                    SetItem(item);
            }
        }
        //Debug.LogError($"Current items: {string.Join(", ", GetSelectedItems().Select(i => i.GetName() + " t=" + i.Type))}");
    }

    public void SetItem(ItemInfo item)
    {
        if (item.GetIsCreated())
            itemSlots.First(i => i.SlotType == item.Type).SetItem(item);
    }

    public void Fade(float fade, float duration)
    {
        _fadeTween.SafeKill();
        _fadeTween = portraitImage.DOFade(fade, duration)
                                    .SetEase(Ease.Linear)
                                    .SetUpdate(UpdateType.Normal);
    }

    public void ForceFade(float fade)
    {
        _fadeTween.SafeKill();
        portraitImage.color = new Color(1, 1, 1, fade);
    }


    private void SetItemsVisible(bool itemVisible, bool hairVisible)
    {
        foreach (var itemslot in itemSlots)
        {
            itemslot.SetVisible(itemslot.SlotType == ItemType.Hair ? hairVisible : itemVisible);
        }
    }


    void Update()
    {
        mainMask.alpha = portraitImage.color.a;
    }


}
