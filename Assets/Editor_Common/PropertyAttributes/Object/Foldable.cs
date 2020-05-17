using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class Foldable : PropertyAttribute
{
    private bool canBeDisabled;
    private string disablingBoolAttribute;

    public Foldable(bool canBeDisabled = false, string disablingBoolAttribute = "")
    {
        this.canBeDisabled = canBeDisabled;
        this.disablingBoolAttribute = disablingBoolAttribute;
    }

    public bool CanBeDisabled { get => canBeDisabled; }
    public string DisablingBoolAttribute { get => disablingBoolAttribute; }
}
