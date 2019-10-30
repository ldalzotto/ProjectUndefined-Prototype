using System;
using OdinSerializer;
using UnityEngine;

namespace SelectionWheel
{
    [Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfigurationData", menuName = "Configuration/PuzzleGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfigurationData", order = 1)]
    public class SelectionWheelNodeConfigurationData : SerializedScriptableObject
    {
        public string DescriptionText;
        public Sprite WheelNodeIcon;
    }
}