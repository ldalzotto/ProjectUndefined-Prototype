using UnityEditor;
using UnityEngine;

public class ShowOnKeywordDrawer : MaterialPropertyDrawer
{
    private string keywordName;

    public ShowOnKeywordDrawer(string keywordName)
    {
        this.keywordName = keywordName;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        var material = (Material)editor.target;
        if (material.IsKeywordEnabled(this.keywordName))
        {
            EditorGUI.indentLevel += 1;
            editor.DefaultShaderProperty(prop, label);
            EditorGUI.indentLevel -= 1;
        }
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0f;
    }

}
