using System;
using System.Collections.Generic;
using System.Linq;
using Editor_MainGameCreationWizard;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor_GameDesigner
{
    [Serializable]
    public abstract class BaseModuleWizardModule<T, ABSTRACT_MODULE> : IGameDesignerModule where T : Object
    {
        [SerializeField] protected bool add;

        [SerializeField] private List<Type> AvailableModules;
        [NonSerialized] protected CommonGameConfigurations CommonGameConfigurations;
        protected GameObject currentSelectedObjet;

        [SerializeField] protected GameDesignerEditorProfile GameDesignerEditorProfile;

        protected SerializedObject GameDesignerEditorProfileSO;
        [SerializeField] protected bool remove;

        [SerializeField] private Vector2 scrollPosition;
        [SerializeField] private int selectedModuleIndex;

        public bool Add => add;

        public bool Remove => remove;

        public GameObject CurrentSelectedObjet => currentSelectedObjet;

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            if (CommonGameConfigurations == null)
            {
                CommonGameConfigurations = new CommonGameConfigurations();
                EditorInformationsHelper.InitProperties(ref CommonGameConfigurations);
            }

            if (this.GameDesignerEditorProfile == null) this.GameDesignerEditorProfile = GameDesignerEditorProfile;

            if (GameDesignerEditorProfileSO == null) GameDesignerEditorProfileSO = new SerializedObject(this.GameDesignerEditorProfile);

            currentSelectedObjet = GameDesignerHelper.GetCurrentSceneSelectedObject();
            selectedModuleIndex = EditorGUILayout.Popup(selectedModuleIndex, AvailableModules.ConvertAll(t => t.Name).ToArray());
            EditorGUILayout.HelpBox(POIModuleDescription(AvailableModules[selectedModuleIndex]), MessageType.None);

            T selectedPointOfIterestType = null;
            if (currentSelectedObjet != null) selectedPointOfIterestType = currentSelectedObjet.GetComponent<T>();

            var additionalEditAllowed = AdditionalEditCondition(AvailableModules[selectedModuleIndex]);

            EditorGUILayout.BeginHorizontal();
            var newAdd = GUILayout.Toggle(add, "ADD", EditorStyles.miniButtonLeft);
            var newRemove = GUILayout.Toggle(remove, "REMOVE", EditorStyles.miniButtonRight);

            if (newAdd && newRemove)
            {
                if (add && !remove)
                {
                    add = false;
                    remove = true;
                }
                else if (!add && remove)
                {
                    add = true;
                    remove = false;
                }
                else
                {
                    add = newAdd;
                    remove = newRemove;
                }
            }
            else
            {
                add = newAdd;
                remove = newRemove;
            }

            EditorGUILayout.EndHorizontal();


            EditorGUI.BeginDisabledGroup(IsDisabled() || !additionalEditAllowed);
            if (GUILayout.Button("EDIT"))
            {
                OnEnabled();
                OnEdit(selectedPointOfIterestType, AvailableModules[selectedModuleIndex]);
                EditorUtility.SetDirty(currentSelectedObjet);
            }

            EditorGUI.EndDisabledGroup();

            if (currentSelectedObjet != null && selectedPointOfIterestType != null) DoModuleListing(selectedPointOfIterestType);

            GameDesignerEditorProfileSO.ApplyModifiedProperties();
            GameDesignerEditorProfileSO.Update();
        }

        public void OnEnabled()
        {
            AvailableModules = TypeHelper.GetAllTypeAssignableFrom(typeof(ABSTRACT_MODULE)).ToList().Select(m => m).Where(m => !m.IsAbstract).ToList();
        }

        public void OnDisabled()
        {
        }

        protected abstract void OnEdit(T RootModuleObject, Type selectedType);
        protected abstract string POIModuleDescription(Type selectedType);
        protected abstract List<ABSTRACT_MODULE> GetModules(T RootModuleObject);

        protected virtual bool AdditionalEditCondition(Type selectedType)
        {
            return true;
        }

        private bool IsDisabled()
        {
            return currentSelectedObjet == null || currentSelectedObjet.GetComponent<T>() == null;
        }

        private void DoModuleListing(T pointOfInterestType)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("POI Modules : ");
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var foundedModules = GetModules(pointOfInterestType);

            EditorGUILayout.BeginVertical();
            if (foundedModules != null)
                foreach (var foundedModule in foundedModules)
                    if (GUILayout.Button(new GUIContent(foundedModule.GetType().Name, POIModuleDescription(foundedModule.GetType()))))
                        selectedModuleIndex = AvailableModules.IndexOf(foundedModule.GetType());

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}