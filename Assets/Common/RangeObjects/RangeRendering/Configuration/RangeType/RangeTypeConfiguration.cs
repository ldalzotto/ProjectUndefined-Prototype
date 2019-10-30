using System;
using ConfigurationEditor;
using UnityEngine;

namespace RangeObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "RangeTypeConfiguration", menuName = "Configuration/PuzzleGame/RangeTypeConfiguration/RangeTypeConfiguration", order = 1)]
    public class RangeTypeConfiguration : ConfigurationSerialization<RangeTypeID, RangeTypeInherentConfigurationData>
    {
    }
}