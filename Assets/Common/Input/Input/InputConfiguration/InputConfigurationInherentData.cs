using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [Serializable]
    [CreateAssetMenu(fileName = "InputConfigurationInherentData", menuName = "Configuration/CoreGame/InputConfiguration/InputConfigurationInherentData", order = 1)]
    public class InputConfigurationInherentData : SerializedScriptableObject
    {
        [ReorderableListAttribute] public List<Key> AttributedKeys;
        [ReorderableListAttribute] public List<MouseButton> AttributedMouseButtons;
        public MouseScroll AttributedMouseScroll;
        public bool Down;
        public bool DownHold;
        public bool Released;
    }
}