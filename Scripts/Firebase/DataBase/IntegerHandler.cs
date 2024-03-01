using System;

public class IntegerHandler
{
    public event Action<int> OnChangeValue;
    public event Action<int, int> OnChangeValueDirection;

    public readonly string Name;

    public bool IsLoaded
    {
        get;
        private set;
    }

    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (_value != value)
            {
                int last = _value;
                _value = value;
                OnChangeValueDirection?.Invoke(_value, _value - last);
                OnChangeValue?.Invoke(_value);
                Save();
            }
        }
    }

    private int _value;
    private int _defaultValue;
    private Action<string, int> _saveDelegate;
    private Action<string, Action<int>, int> _loadDelegate;

    public IntegerHandler(string name, int defaultValue, Action<string, int> saveDelegate, Action<string, Action<int>, int> loadDelegate)
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

    private void OnValueLoaded(int loadedValue)
    {
        if (_value != loadedValue)
        {
            _value = loadedValue;
            OnChangeValue?.Invoke(_value);
        }
        IsLoaded = true;
    }
}
