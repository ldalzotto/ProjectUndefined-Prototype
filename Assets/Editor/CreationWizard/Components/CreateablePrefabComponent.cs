using System;
using UnityEditor;
using UnityEngine;

public abstract class CreateablePrefabComponent<S> : CreationModuleComponent, ICreateable where S : UnityEngine.Object
{
    private GUIStyle selectionStyle;

    [SerializeField]
    private bool selectionToggle;

    private GUIStyle newStyle;

    [SerializeField]
    private bool newToggle;

    [SerializeField]
    private S createdPrefab;

    private GeneratedPrefabAssetManager<S> generatedPrefabAssetManager;

    private S BasePrefab;

    private Editor prefabObjectEditor;
    private UnityEngine.Object lastFrameObject;

    public abstract Func<AbstractCreationWizardEditorProfile, S> BasePrefabProvider { get; }
    public S CreatedPrefab { get => createdPrefab; }

    protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
    {
        if (selectionStyle == null)
        {
            selectionStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        }
        if (newStyle == null)
        {
            newStyle = new GUIStyle(EditorStyles.miniButtonRight);
        }

        EditorGUILayout.BeginHorizontal();
        this.selectionToggle = GUILayout.Toggle(this.selectionToggle, new GUIContent("S"), selectionStyle, GUILayout.Width(20f));
        if (this.selectionToggle)
        {
            this.newToggle = false;
        }
        this.newToggle = GUILayout.Toggle(this.newToggle, new GUIContent("N"), newStyle, GUILayout.Width(20f));
        if (this.newToggle)
        {
            this.selectionToggle = false;
        }
        EditorGUILayout.EndHorizontal();

        if (this.selectionToggle)
        {
            this.createdPrefab = EditorGUILayout.ObjectField("Select " + typeof(S).Name, this.createdPrefab, typeof(S), false) as S;
        }
        else if (this.newToggle)
        {

            this.InstanciateInEditor(editorProfile);
            if (this.lastFrameObject == null || this.lastFrameObject != this.createdPrefab)
            {
                this.prefabObjectEditor = DynamicEditorCreation.Get().CreateEditor(this.createdPrefab);
            }
            if (this.prefabObjectEditor != null)
            {
                this.prefabObjectEditor.OnInspectorGUI();
            }
        }
    }

    public void InstanciateInEditor(AbstractCreationWizardEditorProfile editorProfile)
    {
        this.newToggle = true;
        this.selectionToggle = false;
        if (this.createdPrefab == null && this.BasePrefabProvider != null)
        {
            if (this.BasePrefab == null)
            {
                this.BasePrefab = this.BasePrefabProvider.Invoke(editorProfile);
            }
            if (typeof(S).IsAssignableFrom(typeof(GameObject)))
            {
                this.createdPrefab = PrefabUtility.LoadPrefabContents(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.BasePrefab)) as S;
            }
            else
            {
                this.createdPrefab = PrefabUtility.LoadPrefabContents(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.BasePrefab)).GetComponent<S>();
            }

        }
    }

    public bool IsNew()
    {
        return this.newToggle;
    }
    public bool IsSelected()
    {
        return this.selectionToggle;
    }

    public S Create(string basePath, string baseName, AbstractCreationWizardEditorProfile editorProfile)
    {
        if (this.IsNew())
        {
            this.generatedPrefabAssetManager = new GeneratedPrefabAssetManager<S>(this.BasePrefab, basePath, baseName);
            DestroyImmediate(this.createdPrefab);
            if (typeof(S).IsAssignableFrom(typeof(GameObject)))
            {
                this.createdPrefab = this.generatedPrefabAssetManager.SavedAsset as S;
            }
            else
            {
                this.createdPrefab = this.generatedPrefabAssetManager.SavedAsset.GetComponent<S>();
            }
            editorProfile.AddToGeneratedObjects(this.createdPrefab);
        }
        return this.createdPrefab;
    }

    public void MoveGeneratedAsset(string targetPath)
    {
        if (this.IsNew())
        {
            this.generatedPrefabAssetManager.MoveGeneratedAsset(targetPath);
        }
    }

    public override void ResetEditor()
    {
        this.createdPrefab = null;
    }

    public override void OnGenerationEnd()
    {
        this.newToggle = false;
        this.selectionToggle = true;
    }


}

public interface CreatablePrefabInput
{
}