using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using UnityEngine;

[Serializable]
public class EditorPersistantBoolDatabase : SerializedScriptableObject
{
    public const string EditorPersistantBoolDatabasePath = "Assets/Editor/EditorPersistance/EditorPersistantBoolVariable";

    [SerializeField] private Dictionary<string, bool> Values = new Dictionary<string, bool>();

    public void Persist(string key, bool value)
    {
        this.Values[key] = value;
        EditorUtility.SetDirty(this);
    }

    public void Clear()
    {
        this.Values.Clear();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public bool Get(string key)
    {
        this.Values.TryGetValue(key, out bool value);
        return value;
    }
}