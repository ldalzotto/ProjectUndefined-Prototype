using System;
using ConfigurationEditor;
using LevelManagement;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfiguration", order = 1)]
    public class LevelConfiguration : ConfigurationSerialization<LevelZonesID, LevelConfigurationData>
    {
    }
}