using System;

public class BoolVariable
{
    private bool Value;

    private Action OnJustSetToTrue;
    private Action OnJustSetToFalse;

    public BoolVariable(bool initialValue, Action onJustSetToTrue = null, Action onJustSetToFalse = null)
    {
        Value = initialValue;
        OnJustSetToTrue = onJustSetToTrue;
        OnJustSetToFalse = onJustSetToFalse;
    }

    public void SetValue(bool value)
    {
        bool hasChanged = this.Value != value;
        this.Value = value;
        if (hasChanged)
        {
            if (value)
            {
                this.OnJustSetToTrue?.Invoke();
            }
            else
            {
                this.OnJustSetToFalse?.Invoke();
            }
        }
    }

    public bool GetValue()
    {
        return this.Value;
    }

    public void ReplaceOnJustSetToTrueAction(Action onJustSetToTrue)
    {
        this.OnJustSetToTrue = onJustSetToTrue;
    }
}