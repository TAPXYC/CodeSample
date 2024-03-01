using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using Tapxyc.Attributes;
#endif

public class ThemeChange : MonoBehaviour
{
    [SerializeField] List<ColorChanger> colorChangers;
    [SerializeField] List<TextChanger> textChangers;
    [SerializeField] List<GradientChanger> gradientChangers;
    [SerializeField] List<SpriteChanger> spriteChangers;

    private IEnumerable<BaseThemeChanger> _allChangers => colorChangers.Select(cc => cc as BaseThemeChanger)
                .Union(textChangers)
                .Union(gradientChangers)
                .Union(spriteChangers);


    void OnEnable()
    {
        ChangeTheme(SettingsMenu.CurrentTheme);
        SettingsMenu.OnThemeChange += ChangeTheme;
    }


    void OnDisable()
    {
        SettingsMenu.OnThemeChange -= ChangeTheme;
    }


    public void ChangeTheme(ColorTheme theme)
    {
        foreach (var cc in _allChangers)
        {
            cc.ChangeTheme(theme);
        }
    }



#if UNITY_EDITOR
    [EditorButton("Set light")]
    public void SetLight()
    {
        EditorChangeTheme(ColorTheme.Light);
    }

    [EditorButton("Set dark")]
    public void SetDark()
    {
        EditorChangeTheme(ColorTheme.Dark);
    }

    public void EditorChangeTheme(ColorTheme theme)
    {
        foreach (var cc in _allChangers)
        {
            cc.ChangeTheme(theme);
        }
    }
#endif


    [Serializable]
    abstract class BaseThemeChanger
    {
        private ColorTheme? _currentTheme;
        protected abstract void OnChangeTheme(ColorTheme theme);

        public void ChangeTheme(ColorTheme theme)
        {
            if (_currentTheme == null || _currentTheme.Value != theme)
            {
                OnChangeTheme(theme);
                _currentTheme = theme;
            }
        }




#if UNITY_EDITOR

        public void EditorChangeTheme(ColorTheme theme)
        {
            OnChangeTheme(theme);
        }

#endif
    }




    [Serializable]
    class ColorChanger : BaseThemeChanger
    {
        [SerializeField] ThemeColors colorThemes;
        [SerializeField] List<Image> imagesList;

        protected override void OnChangeTheme(ColorTheme theme)
        {
            if (colorThemes != null)
            {
                var ourTheme = colorThemes.Colors.FirstOrDefault(x => x.Theme == theme);
                if (ourTheme != null)
                {
                    foreach (var i in imagesList)
                    {
                        if (i != null)
                            i.color = ourTheme.Data;
                    }
                }
            }
        }
    }



    [Serializable]
    class TextChanger : BaseThemeChanger
    {
        [SerializeField] ThemeColors colorThemes;
        [SerializeField] List<TMP_Text> textsList;

        protected override void OnChangeTheme(ColorTheme theme)
        {
            if (colorThemes != null)
            {
                var ourTheme = colorThemes.Colors.FirstOrDefault(x => x.Theme == theme);
                if (ourTheme != null)
                {
                    foreach (var t in textsList)
                    {
                        if (t != null)
                            t.color = ourTheme.Data;
                    }
                }
            }
        }
    }



    [Serializable]
    class GradientChanger : BaseThemeChanger
    {
        [SerializeField] ThemeGradient gradientThemes;
        [SerializeField] List<UIGradient> gradientsList;

        protected override void OnChangeTheme(ColorTheme theme)
        {
            if (gradientsList != null)
            {
                var ourTheme = gradientThemes.Gradients.FirstOrDefault(x => x.Theme == theme);
                if (ourTheme != null)
                {
                    foreach (var t in gradientsList)
                    {
                        if (t != null)
                        {
                            t.m_color1 = ourTheme.Data.colorKeys.First().color;
                            t.m_color2 = ourTheme.Data.colorKeys.Last().color;
                            t.ApplyMesh();
                        }
                    }
                }
            }
        }
    }


    [Serializable]
    class SpriteChanger : BaseThemeChanger
    {
        [SerializeField] ThemeSprites spriteThemes;
        [SerializeField] List<Image> imagesList;

        protected override void OnChangeTheme(ColorTheme theme)
        {
            if (spriteThemes != null)
            {
                var ourTheme = spriteThemes.Sprites.FirstOrDefault(x => x.Theme == theme);
                if (ourTheme != null)
                {
                    foreach (var i in imagesList)
                    {
                        if (i != null)
                            i.sprite = ourTheme.Data;
                    }
                }
            }
        }
    }
}


[Serializable]
public class ThemeInfo<T>
{
    public ColorTheme Theme;
    public T Data;
}