using System;
using LevelManagement_Interfaces;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfigurationData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfigurationDataData", order = 1)]
    public class LevelConfigurationData : ScriptableObject
    {
        [SerializeField] public LevelRangeEffectInherentData LevelRangeEffectInherentData;
    }
}