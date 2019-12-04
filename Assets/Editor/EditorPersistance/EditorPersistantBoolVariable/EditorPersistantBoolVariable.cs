using UnityEditor;
using UnityEngine;

public class EditorPersistantBoolVariable
{
    public static EditorPersistantBoolDatabase EditorPersistantBoolDatabase;

    private bool Value;

    private string key;

    public static string BuildKeyFromObject(Object obj, string foldoutName)
    {
        return obj.GetInstanceID() + "_" + foldoutName;
    }

    public static void Initialize(ref EditorPersistantBoolVariable EditorPersistantBoolVariable, string key)
    {
        if (EditorPersistantBoolVariable == null)
        {
            EditorPersistantBoolVariable = new EditorPersistantBoolVariable(key);
        }
    }
    
    public EditorPersistantBoolVariable(string key)
    {
        this.key = key;

        this.Value = GetValue();
    }

    public void SetValue(bool value)
    {
        if (EditorPersistantBoolDatabase == null)
        {
            EditorPersistantBoolDatabase = AssetFinder.SafeSingleAssetFind<EditorPersistantBoolDatabase>("t:" + typeof(EditorPersistantBoolDatabase));
            EditorApplication.wantsToQuit += () =>
            {
                EditorPersistantBoolDatabase.Clear();
                return true;
            };
        }

        if ((!this.Value && value) || (this.Value && !value))
        {
            EditorPersistantBoolDatabase.Persist(key, value);
        }

        this.Value = value;
    }

    public bool GetValue()
    {
        if (EditorPersistantBoolDatabase == null)
        {
            EditorPersistantBoolDatabase = AssetFinder.SafeSingleAssetFind<EditorPersistantBoolDatabase>("t:" + typeof(EditorPersistantBoolDatabase));
        }

        return EditorPersistantBoolDatabase.Get(key);
    }
}