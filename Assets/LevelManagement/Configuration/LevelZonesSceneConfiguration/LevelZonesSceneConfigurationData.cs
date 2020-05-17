using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace LevelManagement
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelZonesSceneConfigurationData", menuName = "Configuration/CoreGame/LevelZonesSceneConfiguration/LevelZonesSceneConfigurationData", order = 1)]
    public class LevelZonesSceneConfigurationData : ScriptableObject, ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        public SceneAsset scene;
#endif
        public string sceneName;

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (this.scene != null)
            {
                this.sceneName = this.scene.name;
            }
#endif
        }
    }
}