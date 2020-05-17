using UnityEngine;
using System.Collections;
using UnityEditor;

public class FileUtil
{

#if UNITY_EDITOR
    public static string GetAssetDirectoryPath(UnityEngine.Object asset)
    {
        var graphTmpFolderFullPath = string.Empty;
        var fullProfilePath = AssetDatabase.GetAssetPath(asset);
        var splittedPaths = fullProfilePath.Split('/');
        for (var i = 0; i < splittedPaths.Length - 1; i++)
        {
            graphTmpFolderFullPath += splittedPaths[i];
            graphTmpFolderFullPath += "/";
        }
        return graphTmpFolderFullPath;
    }
#endif
}
