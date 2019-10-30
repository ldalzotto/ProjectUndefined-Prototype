using System;
using ConfigurationEditor;
using UnityEngine;

namespace SelectionWheel
{
    [Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfiguration", menuName = "Configuration/CoreGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfiguration", order = 1)]
    public class SelectionWheelNodeConfiguration : ConfigurationSerialization<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>
    {
    }
}