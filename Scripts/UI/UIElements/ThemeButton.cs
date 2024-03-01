using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite[] icon = new Sprite[2];

    private int _currentIndexTheme = 0;


    public void Init()
    {
        UpdateThemeIndex();
        ChangeSprite(PlayerPrefs.GetInt("theme", 0));
    }


    public void ButtonClick()
    {
        ChangeThemeIndex();
        ChangeTheme(_currentIndexTheme);
        ChangeSprite(_currentIndexTheme);
    }


    void ChangeSprite(int currentIndexTheme)
    {
        image.sprite = icon[currentIndexTheme];
    }


    void ChangeTheme(int indexTheme)
    {
        SettingsMenu.ChangeTheme((ColorTheme)indexTheme);
    }


    void UpdateThemeIndex()
    {
        _currentIndexTheme = PlayerPrefs.GetInt("theme", 0);
    }


    void ChangeThemeIndex()
    {
        UpdateThemeIndex();
        if (_currentIndexTheme == (int)ColorTheme.Light)
        {
            PlayerPrefs.SetInt("theme", (int)ColorTheme.Dark);
            UpdateThemeIndex();
        }
        else if (_currentIndexTheme == (int)ColorTheme.Dark)
        {
            PlayerPrefs.SetInt("theme", (int)ColorTheme.Light);
            UpdateThemeIndex();
        }
    }
}
