using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetHelper
{
    public static ScriptableObject CreateAssetAtSameDirectoryLevel(ScriptableObject initialAsset, Type typeToInstanciate, string assetName)
    {
        return CreateAssetAtSameDirectoryLevel(initialAsset, typeToInstanciate.Name, assetName);
    }

    public static ScriptableObject CreateAssetAtSameDirectoryLevel(ScriptableObject initialAsset, string typeToInstanciate, string assetName)
    {
        var baseAssetPath = AssetDatabase.GetAssetPath(initialAsset);
        var createdObject = ScriptableObject.CreateInstance(typeToInstanciate);
        AssetDatabase.CreateAsset(createdObject, Path.GetDirectoryName(baseAssetPath) + "/" + Path.GetFileNameWithoutExtension(baseAssetPath) + "_" + assetName + ".asset");
        return createdObject;
    }

    public static string GetAssetPath(UnityEngine.Object Object, bool fileNameIncluded = false)
    {
        var assetPath = AssetDatabase.GetAssetPath(Object);
        if (!fileNameIncluded)
        {
            var splittedPath = assetPath.Split('/');
            assetPath = string.Empty;
            for (var i = 0; i < splittedPath.Length - 1; i++)
            {
                assetPath += splittedPath[i];
                if (i != (splittedPath.Length - 1))
                {
                    assetPath += "/";
                }
            }
        }
        return assetPath;
    }
}
