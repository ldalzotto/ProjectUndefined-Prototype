using System;
using ConfigurationEditor;
using UnityEngine;

namespace Input
{
    [Serializable]
    [CreateAssetMenu(fileName = "InputConfiguration", menuName = "Configuration/CoreGame/InputConfiguration/InputConfiguration", order = 1)]
    public class InputConfiguration : ConfigurationSerialization<InputID, InputConfigurationInherentData>
    {
    }
}