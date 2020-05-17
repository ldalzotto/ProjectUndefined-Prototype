using System.IO;
using UnityEditor;
using UnityEngine;

public class PersistanceConfigurationTool : EditorWindow
{
    [MenuItem("Configuration/PersistanceConfigurationTool")]
    static void Init()
    {
        PersistanceConfigurationTool window = (PersistanceConfigurationTool)EditorWindow.GetWindow(typeof(PersistanceConfigurationTool));
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("DELETE"))
        {
            var persistanceDirectory = new DirectoryInfo(Application.persistentDataPath);
            foreach(var directory in persistanceDirectory.GetDirectories())
            {
                directory.Delete(true);
            }
        }
    }
}
