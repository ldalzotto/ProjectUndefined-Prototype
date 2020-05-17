using System;
using OdinSerializer;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = "GlobalLevelConfiguration", menuName = "Configuration/CoreGame/GlobalLevelConfiguration/GlobalLevelConfiguration", order = 1)]
    public class GlobalLevelConfiguration : SerializedScriptableObject
    {
        [CustomEnum()] public LevelZonesID NewGameStartLevelID;
    }
}