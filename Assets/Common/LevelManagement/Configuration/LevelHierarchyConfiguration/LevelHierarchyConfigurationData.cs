using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelHierarchyConfigurationData", menuName = "Configuration/CoreGame/LevelHierarchyConfiguration/LevelHierarchyConfigurationData", order = 1)]
    public class LevelHierarchyConfigurationData : ScriptableObject
    {
        [ReorderableListAttribute] public List<LevelZoneChunkID> LevelHierarchy;
    }
}