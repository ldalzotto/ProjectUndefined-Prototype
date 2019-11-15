using System;
using System.Collections.Generic;
using UnityEditor;

public class ObjectVariable<T>
{
    private T Value;
    
    #region Callbacks

    public delegate void OnObjectValueChangedDelgate(T Old, T New);
    private OnObjectValueChangedDelgate OnObjectValueChanged;

    private Func<T, T, bool> EqualityFunction;
    #endregion

    public ObjectVariable(OnObjectValueChangedDelgate OnObjectValueChanged = null, Func<T, T, bool> Equality = null)
    {
        this.OnObjectValueChanged = OnObjectValueChanged;
        if (Equality == null)
        {
            this.EqualityFunction = (v1, v2) => EqualityComparer<T>.Default.Equals(v1, v2);
        }
    }

    public void SetValue(T newValue)
    {
        var oldValue = this.Value;
        this.Value = newValue;
        if (!this.EqualityFunction(oldValue, newValue))
        {
            this.OnObjectValueChanged?.Invoke(oldValue, newValue);
        }

    }

    public T GetValue()
    {
        return this.Value;
    }


}