using System.Collections.Generic;
using CoreGame;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Input
{
    public class InputControlLookup : GameSingleton<InputControlLookup>
    {
        public Dictionary<Key, KeyControl> keyToKeyControlLookup { get; private set; }
        public Dictionary<MouseButton, ButtonControl> mouseButtonControlLookup { get; private set; }
        public Dictionary<MouseScroll, Vector2Control> mouseScrollControlLookup { get; private set; }

        public InputControlLookup()
        {
            this.keyToKeyControlLookup = new Dictionary<Key, KeyControl>();
            this.mouseButtonControlLookup = new Dictionary<MouseButton, ButtonControl>();
            this.mouseScrollControlLookup = new Dictionary<MouseScroll, Vector2Control>();

            foreach (var keyBoardKeyControl in Keyboard.current.allKeys)
            {
                this.keyToKeyControlLookup[keyBoardKeyControl.keyCode] = keyBoardKeyControl;
            }

            this.mouseButtonControlLookup[MouseButton.LEFT_BUTTON] = Mouse.current.leftButton;
            this.mouseButtonControlLookup[MouseButton.RIGHT_BUTTON] = Mouse.current.rightButton;
            this.mouseScrollControlLookup[MouseScroll.SCROLL] = Mouse.current.scroll;
        }

        public static string GetInputControlRawName(InputControl inputControl)
        {
            string returnValue = string.Empty;
            if (!string.IsNullOrEmpty(inputControl.shortDisplayName) && inputControl.shortDisplayName != "Null")
            {
                returnValue = inputControl.shortDisplayName;
            }
            else if (!string.IsNullOrEmpty(inputControl.displayName) && inputControl.displayName != "Null")
            {
                returnValue = inputControl.displayName;
            }
            else if (!string.IsNullOrEmpty(inputControl.name) && inputControl.name != "Null")
            {
                returnValue = inputControl.name;
            }
            else
            {
                returnValue = string.Empty;
            }

            return returnValue;
        }

        /// <summary>
        /// For the input <paramref name="InputID"/>. Returns the first <see cref="InputControl"/> recovered from <see cref="InputConfigurationInherentData"/>.
        /// Check is done in this order :
        ///     * <see cref="InputConfigurationInherentData.AttributedKeys"/>
        ///     * <see cref="InputConfigurationInherentData.AttributedMouseButtons"/>
        /// </summary>
        public static InputControl FindTheFirstInputControlForInputID(InputID InputID)
        {
            var InputConfigurationInherentData = InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[InputID];
            if (InputConfigurationInherentData != null)
            {
                var inputControlLookup = InputControlLookup.Get();
                if (InputConfigurationInherentData.AttributedKeys != null)
                {
                    foreach (var attributedKey in InputConfigurationInherentData.AttributedKeys)
                    {
                        var inputControl = inputControlLookup.keyToKeyControlLookup[attributedKey];
                        if (inputControl != null)
                        {
                            return inputControl;
                        }
                    }
                }

                if (InputConfigurationInherentData.AttributedMouseButtons != null)
                {
                    foreach (var attributedMouseButton in InputConfigurationInherentData.AttributedMouseButtons)
                    {
                        var inputControl = inputControlLookup.mouseButtonControlLookup[attributedMouseButton];
                        if (inputControl != null)
                        {
                            return inputControl;
                        }
                    }
                }
            }

            return null;
        }
    }
}