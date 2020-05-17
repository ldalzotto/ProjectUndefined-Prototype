using System;
using System.Collections.Generic;
using ConfigurationEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LevelManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelHierarchyConfiguration", menuName = "Configuration/CoreGame/LevelHierarchyConfiguration/LevelHierarchyConfiguration", order = 1)]
    public class LevelHierarchyConfiguration : ConfigurationSerialization<LevelZonesID, LevelHierarchyConfigurationData>
    {
        public List<LevelZoneChunkID> GetLevelHierarchy(LevelZonesID levelZonesID)
        {
            if (!ConfigurationInherentData.ContainsKey(levelZonesID) || ConfigurationInherentData[levelZonesID] == null)
            {
                return new List<LevelZoneChunkID>();
            }
            else
            {
                return ConfigurationInherentData[levelZonesID].LevelHierarchy;
            }
        }

#if UNITY_EDITOR
        public void AddPuzzleChunkLevel(LevelZonesID levelZonesID, LevelZoneChunkID levelZoneChunkID)
        {
            var foundInherentData = this.ConfigurationInherentData[levelZonesID];
            if (foundInherentData.LevelHierarchy == null)
            {
                foundInherentData.LevelHierarchy = new List<LevelZoneChunkID>();
            }

            if (!foundInherentData.LevelHierarchy.Contains(levelZoneChunkID))
            {
                foundInherentData.LevelHierarchy.Add(levelZoneChunkID);
                EditorUtility.SetDirty(foundInherentData);
            }
        }

        public void RemovePuzzleChunkLevel(LevelZonesID levelZonesID, LevelZoneChunkID levelZoneChunkID)
        {
            var foundInherentData = this.ConfigurationInherentData[levelZonesID];
            if (foundInherentData.LevelHierarchy == null)
            {
                foundInherentData.LevelHierarchy = new List<LevelZoneChunkID>();
            }

            if (foundInherentData.LevelHierarchy.Contains(levelZoneChunkID))
            {
                foundInherentData.LevelHierarchy.Remove(levelZoneChunkID);
                EditorUtility.SetDirty(foundInherentData);
            }
        }
#endif
    }
}