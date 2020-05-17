#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GeneratedScriptableObjectManager<T> where T : ScriptableObject
{
    private string generatedFileName;
    private T generatedAsset;

    public GeneratedScriptableObjectManager(T originalObject, string folderPath, string fileBaseName)
    {
        this.generatedFileName = "\\" + fileBaseName + ".asset";
        AssetDatabase.CreateAsset(originalObject, folderPath + this.generatedFileName);
        this.generatedAsset = originalObject;

    }

    public T GeneratedAsset { get => generatedAsset; }

    public void MoveGeneratedAsset(string targetPath)
    {
        var oldPath = AssetDatabase.GetAssetPath(this.generatedAsset);
        var newPath = targetPath + this.generatedFileName;
        var errorMessage = AssetDatabase.ValidateMoveAsset(oldPath, newPath);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            throw new AssetMoveError(errorMessage);
        }
        AssetDatabase.MoveAsset(oldPath, newPath);
    }
}
#endif