using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InputConfigurationInherentData", menuName = "Configuration/CoreGame/InputConfiguration/InputConfigurationInherentData", order = 1)]
    public class InputConfigurationInherentData : ScriptableObject
    {
        [ReorderableListAttribute] public List<Key> AttributedKeys;
        [ReorderableListAttribute] public List<MouseButton> AttributedMouseButtons;
        public MouseScroll AttributedMouseScroll;
        public bool Down;
        public bool DownHold;

        public Key GetAssociatedInputKey()
        {
            if (this.AttributedKeys != null && this.AttributedKeys.Count > 0)
            {
                return this.AttributedKeys[0];
            }

            return Key.None;
        }

        public MouseButton GetAssociatedMouseButton()
        {
            if (this.AttributedMouseButtons != null && this.AttributedMouseButtons.Count > 0)
            {
                return this.AttributedMouseButtons[0];
            }

            return MouseButton.NONE;
        }

        public MouseScroll GetAssociatedMouseScroll()
        {
            return this.AttributedMouseScroll;
        }
    }
}