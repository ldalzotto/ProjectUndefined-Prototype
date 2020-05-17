using System;
using ConfigurationEditor;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelZonesSceneConfiguration", menuName = "Configuration/CoreGame/LevelZonesSceneConfiguration/LevelZonesSceneConfiguration", order = 1)]
    public class LevelZonesSceneConfiguration : ConfigurationSerialization<LevelZonesID, LevelZonesSceneConfigurationData>
    {
        public string GetSceneName(LevelZonesID levelZonesID)
        {
            return ConfigurationInherentData[levelZonesID].sceneName;
        }

#if UNITY_EDITOR
        public string GetSceneNameSafe(LevelZonesID levelZonesID)
        {
            ConfigurationInherentData.TryGetValue(levelZonesID, out LevelZonesSceneConfigurationData configurationData);
            if (configurationData != null)
            {
                return configurationData.sceneName;
            }

            return string.Empty;
        }
#endif
    }
}