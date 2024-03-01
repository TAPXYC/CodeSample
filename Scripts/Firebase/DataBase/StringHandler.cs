using System;

public class StringHandler
{
    public event Action<string> OnChangeValue;

    public readonly string Name;

    public bool IsLoaded
    {
        get;
        private set;
    }

    public string Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (_value != value)
            {
                _value = value;
                OnChangeValue?.Invoke(_value);
                Save();
            }
        }
    }

    private string _value;
    private string _defaultValue;
    private Action<string, string> _saveDelegate;
    private Action<string, Action<string>, string> _loadDelegate;

    public StringHandler(string name, string defaultValue, Action<string, string> saveDelegate, Action<string, Action<string>, string> loadDelegate)
    {
        Name = name;
        _saveDelegate = saveDelegate;
        _loadDelegate = loadDelegate;
        _defaultValue = defaultValue;
        IsLoaded = false;
    }


    public void Save()
    {
        _saveDelegate(Name, _value);
    }

    public void Load()
    {
        IsLoaded = false;
        _loadDelegate(Name, OnValueLoaded, _defaultValue);
    }

    private void OnValueLoaded(string loadedValue)
    {
        if (_value != loadedValue)
        {
            _value = loadedValue;
            OnChangeValue?.Invoke(_value);
        }
        IsLoaded = true;
    }
}
