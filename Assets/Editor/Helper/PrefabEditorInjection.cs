
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class PrefabEditorInjection
{
    private GameObject selectedPrefab;
    private GameObject instanciatedPrefab;

    public GameObject SelectedPrefab { get => selectedPrefab; set => selectedPrefab = value; }

    public void ReloadInstanciatedPrefab()
    {
        if (selectedPrefab != null)
        {
            this.instanciatedPrefab = PrefabUtility.LoadPrefabContents(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.selectedPrefab));
        }
    }

    public void OnObjectFieldSelectionChange()
    {
        if (this.instanciatedPrefab != null) //unload prefab instance on change
        {
            PrefabUtility.UnloadPrefabContents(this.instanciatedPrefab);
        }

        if (this.selectedPrefab != null) //load new prefab instance if selected
        {
            if (PrefabUtility.GetPrefabAssetType(this.selectedPrefab) != PrefabAssetType.NotAPrefab) //if this is a prefab
            {
                this.instanciatedPrefab = PrefabUtility.LoadPrefabContents(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.selectedPrefab));
            }
        }
    }

    public void DisplayComponentEditorUI<T>() where T : Component
    {
        DisplayComponentEditorUI<T>(null);
    }

    public void DisplayComponentEditorUI<T>(Action<T> customDisplayFunction) where T : Component
    {
        if (this.selectedPrefab != null)
        {
            if (this.instanciatedPrefab == null && PrefabUtility.GetPrefabAssetType(this.selectedPrefab) != PrefabAssetType.NotAPrefab)
            {
                this.instanciatedPrefab = PrefabUtility.LoadPrefabContents(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.selectedPrefab));
            }

            if (this.instanciatedPrefab) //new prefab workflow
            {
                var lookedComponent = this.instanciatedPrefab.GetComponent<T>();
                if (lookedComponent != null)
                {
                    EditorGUI.BeginChangeCheck();
                    if (customDisplayFunction != null)
                    {
                        customDisplayFunction.Invoke(lookedComponent);
                    }
                    else
                    {
                        DynamicEditorCreation.Get().CreateEditor(this.instanciatedPrefab.GetComponent<T>()).OnInspectorGUI();
                    }
                    if (EditorGUI.EndChangeCheck()) // We simulate the "auto-save" feature of the new prefab pipeline
                    {
                        PrefabUtility.SaveAsPrefabAsset(this.instanciatedPrefab, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedPrefab));
                    }
                }

            }
            else //fallback to old workflow method
            {
                DynamicEditorCreation.Get().CreateEditor(this.selectedPrefab.GetComponent<Collider>()).OnInspectorGUI();
            }
        }
    }

    public void OnDisable()
    {
        //save and unload on exit
        if (this.instanciatedPrefab != null)
        {
            PrefabUtility.SaveAsPrefabAsset(this.instanciatedPrefab, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedPrefab));
        }
    }


}

#endif