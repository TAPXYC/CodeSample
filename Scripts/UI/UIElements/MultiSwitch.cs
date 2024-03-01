using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ColorTheme
{
    Light,
    Dark
}

public class MultiSwitch : MonoBehaviour
{
    public event Action<int> OnClick;
    [SerializeField] List<string> options;
    [SerializeField] TMP_Text text;

    private string _currentOption;
    private int _currentOptionIndex;
    
    public void Init(string keyOption)
    {
        _currentOptionIndex = PlayerPrefs.GetInt(keyOption, 0);
        _currentOption = options[_currentOptionIndex];
        ShowOption(_currentOptionIndex);
    }

    public void RightClick()
    {
        _currentOptionIndex++;
        if (_currentOptionIndex >= options.Count)
        {
            _currentOptionIndex = 0;
        }
        ShowOption(_currentOptionIndex);
        OnClick?.Invoke(_currentOptionIndex);
    }

    public void LeftClick()
    {
        _currentOptionIndex--;
        if (_currentOptionIndex < 0)
        {
            _currentOptionIndex = options.Count - 1;
        }
        ShowOption(_currentOptionIndex);
        OnClick?.Invoke(_currentOptionIndex);
    }


    private void ShowOption(int i)
    {
        text.text = options[i];
    }

}
