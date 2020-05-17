using ConfigurationEditor;
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IncludeConfiguration))]
public class IncludeConfigurationPropertyDrawer : PropertyDrawer
{
    private FoldableArea FoldableArea;
    private Editor CachedConfigurationEditor;
    private Enum lastFrameEnum;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        IncludeConfiguration IncludeConfigurationEnum = (IncludeConfiguration)attribute;
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            var targetEnum = SerializableObjectHelper.GetBaseProperty<Enum>(property);
            if (this.CachedConfigurationEditor == null || this.lastFrameEnum.ToString() != targetEnum.ToString())
            {
                var configuration = (IConfigurationSerialization)AssetFinder.SafeAssetFind("t:" + IncludeConfigurationEnum.ConfigurationType.Name)[0];
                var so = configuration.GetEntry(targetEnum);
                this.CachedConfigurationEditor = DynamicEditorCreation.Get().CreateEditor(so);
                this.FoldableArea = new FoldableArea(false, so.name, false);
            }

            if (CachedConfigurationEditor != null)
            {
                this.FoldableArea.OnGUI(() =>
                {
                    this.CachedConfigurationEditor.OnInspectorGUI();
                });
            }
            this.lastFrameEnum = targetEnum;
        }


    }
}
