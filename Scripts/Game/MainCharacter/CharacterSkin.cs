using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSkin : MonoBehaviour
{
    [SerializeField] Image image;

    public CharacterSkinSetting Skin => _skin;
    public bool HideItems
    {
        get;
        private set;
    }
    public bool HideHair
    {
        get;
        private set;
    }

    private EmotionType _lastEmotionType;
    private string _lastEmotionID;
    private CharacterSkinSetting _skin;

    public void Init()
    {
        _lastEmotionType = EmotionType.Normal;
    }

    public Sprite SetSkin(CharacterSkinSetting skin)
    {
        if (skin != null && (_skin == null || _skin.SkinID != skin.SkinID))
        {
            _skin = skin;
            bool hideItems;
            bool hideHair;
            image.sprite = _lastEmotionType == EmotionType.Custom ? _skin.GetSkin(_lastEmotionID, out hideItems, out hideHair) : 
                                                                    _skin.GetSkin(_lastEmotionType, out hideItems, out hideHair);
            HideItems = hideItems;
            HideHair = hideHair;
        }

        return image.sprite;
    }


    public Sprite SetEmotion(EmotionType emotion)
    {
        if (_lastEmotionType != emotion)
        {
            _lastEmotionID = string.Empty;

            _lastEmotionType = emotion;
            bool hideItems;
            bool hideHair;
            image.sprite = _skin.GetSkin(_lastEmotionType, out hideItems, out hideHair);
            HideItems = hideItems;
            HideHair = hideHair;
        }
        return image.sprite;
    }

    public Sprite SetEmotion(string emotionID)
    {
        if (_lastEmotionID != emotionID)
        {
            _lastEmotionType = EmotionType.Custom;

            _lastEmotionID = emotionID;
            bool hideItems;
            bool hideHair;
            image.sprite = _skin.GetSkin(_lastEmotionID, out hideItems, out hideHair);
            HideItems = hideItems;
            HideHair = hideHair;
        }
        return image.sprite;
    }
}
