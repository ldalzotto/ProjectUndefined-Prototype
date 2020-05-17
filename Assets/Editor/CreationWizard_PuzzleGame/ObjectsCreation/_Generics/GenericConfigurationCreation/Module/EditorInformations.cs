using ConfigurationEditor;
using CreationWizard;
using Editor_MainGameCreationWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GenericConfigurationCreation
{
    [System.Serializable]
    public class EditorInformations : CreationModuleComponent
    {
        public EditorInformationsData EditorInformationsData;

        private List<Type> AllConfigurationTypes;
        private int selectedTypeIndex;

        public override void ResetEditor()
        {
        }

        public void SetSelectedConf(Type configurationType)
        {
            this.InitProperties();
            this.selectedTypeIndex = this.AllConfigurationTypes.IndexOf(configurationType);
        }

        private void InitProperties()
        {
            if (this.AllConfigurationTypes == null || this.AllConfigurationTypes.Count == 0)
            {
                this.AllConfigurationTypes = TypeHelper.GetAllGameConfigurationTypes().ToList();
            }
            EditorInformationsHelper.InitProperties(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        private Rect dropDownRect = new Rect();

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();

            EditorGUI.BeginChangeCheck();
            this.selectedTypeIndex = EditorGUILayout.Popup(this.selectedTypeIndex, this.AllConfigurationTypes.ConvertAll(t => t.Name).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                EditorInformationsData.SelectedKey = (Enum)Activator.CreateInstance(this.AllConfigurationTypes[this.selectedTypeIndex].BaseType.GetGenericArguments()[0]);
            }

            if (EditorInformationsData.SelectedKey != null)
            {
                var dropwdown = EditorGUILayout.DropdownButton(new GUIContent(EditorInformationsData.SelectedKey.ToString()), FocusType.Keyboard);
                if (Event.current.type == EventType.Repaint)
                {
                    this.dropDownRect = GUILayoutUtility.GetLastRect();
                }

                if (dropwdown)
                {
                    var windowInstance = EditorWindow.CreateInstance<EnumSearchGUIWindow>();
                    windowInstance.Init(EditorInformationsData.SelectedKey, (newSelectedEnum) =>
                    {
                        EditorInformationsData.SelectedKey = newSelectedEnum;
                    });

                    var windowRect = new Rect(GUIUtility.GUIToScreenPoint(dropDownRect.position), new Vector2(0, dropDownRect.height));
                    windowInstance.ShowAsDropDown(windowRect, new Vector2(dropDownRect.width, 500));
                }
            }
            EditorInformationsData.SelectedConfiguration = this.EditorInformationsData.CommonGameConfigurations.GetConfiguration(this.AllConfigurationTypes[this.selectedTypeIndex]);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)).FindPropertyRelative(nameof(EditorInformationsData.CommonGameConfigurations)), true);
        }

        public override string ComputeErrorState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>() {
                 EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations),
                 this.AttractiveObjectModelVerification()
            }.Find((s) => !string.IsNullOrEmpty(s));
        }

        private string AttractiveObjectModelVerification()
        {
            return string.Empty;
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>()
            {
                  ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.SelectedKey, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration(this.AllConfigurationTypes[this.selectedTypeIndex]))
            }.Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [CustomEnum()]
        public Enum SelectedKey;
        public IConfigurationSerialization SelectedConfiguration;
        public CommonGameConfigurations CommonGameConfigurations;
    }
}