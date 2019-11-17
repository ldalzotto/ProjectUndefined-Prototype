using System;

public delegate void OnValueChangedDelegate(float OldValue, float newValue);

public class FloatVariable
{
    private float Value;

    private OnValueChangedDelegate OnValueChanged;

    public FloatVariable(float startValue, OnValueChangedDelegate onValueChanged = null)
    {
        Value = startValue;
        OnValueChanged = onValueChanged;
    }

    public void SetValue(float newValue)
    {
        var oldValue = this.Value;
        this.Value = newValue;
        if (oldValue != newValue)
        {
            this.OnValueChanged?.Invoke(oldValue, newValue);
        }
    }

    public float GetValue()
    {
        return this.Value;
    }
}