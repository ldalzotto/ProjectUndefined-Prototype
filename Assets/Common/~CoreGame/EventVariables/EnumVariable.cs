using System;
using System.Collections.Generic;

public class EnumVariable<T> where T : Enum
{
    private T Value;

    public delegate void OnEnumValueChangedDelegate(T OldValue, T NewValue);

    private OnEnumValueChangedDelegate OnEnumValueChanged;

    public EnumVariable(T startValue, OnEnumValueChangedDelegate onEnumValueChanged = null)
    {
        this.Value = startValue;
        OnEnumValueChanged = onEnumValueChanged;
        this.OnEnumValueChanged?.Invoke(this.Value, this.Value);
    }

    public T GetValue()
    {
        return this.Value;
    }

    public void SetValue(T enumValue)
    {
        var oldValue = this.Value;
        Value = enumValue;
        if (!EqualityComparer<T>.Default.Equals(enumValue, oldValue))
        {
            this.OnEnumValueChanged?.Invoke(oldValue, enumValue);
        }
    }
}