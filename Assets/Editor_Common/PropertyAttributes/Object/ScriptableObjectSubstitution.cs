using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class ScriptableObjectSubstitution : PropertyAttribute
{
    public string SubstitutionName;
    public string SourcePickerName;
    private string sourceChoiceLabel;
    private string substitutionChoiceLabel;

    public ScriptableObjectSubstitution(string substitutionName, string sourcePickerName)
    {
        SubstitutionName = substitutionName;
        SourcePickerName = sourcePickerName;
        this.sourceChoiceLabel = "ID";
        this.substitutionChoiceLabel = "InherentData";
    }

    public string SourceChoiceLabel { get => sourceChoiceLabel; }
    public string SubstitutionChoiceLabel { get => substitutionChoiceLabel; }
}
