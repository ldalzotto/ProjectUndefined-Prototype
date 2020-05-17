using System;
using ConfigurationEditor;
using UnityEngine;

namespace InteractiveObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "InteractiveObjectV2Configuration", menuName = "Configuration/PuzzleGame/InteractiveObjectV2Configuration/InteractiveObjectV2Configuration", order = 1)]
    public class InteractiveObjectV2Configuration : ConfigurationSerialization<InteractiveObjectV2DefinitionID, AbstractInteractiveObjectV2Definition>
    {
    }
}