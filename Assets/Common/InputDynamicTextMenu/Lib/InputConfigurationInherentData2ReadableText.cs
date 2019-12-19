using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using Input;
using UnityEngine.InputSystem;

namespace InputDynamicTextMenu
{
    public static class InputConfigurationInherentData2ReadableText
    {
        public static Dictionary<InputID, string> InputReadableText = new Dictionary<InputID, string>()
        {
            {InputID.CAMERA_ROTATION_DOWN_HOLD, "Camera rotation trigger"},
            {InputID.CAMERA_ZOOM, "Camera zoom"},
            {InputID.FIRING_ACTION_DOWN, "Entering firirng mode"},
            {InputID.FIRING_PROJECTILE_DOWN_HOLD, "Firing"},
            {InputID.DEFLECT_PROJECTILE_DOWN, "Deflection"}
        };

        public static string ConvertInputToReadableText(InputID inputId)
        {
            InputReadableText.TryGetValue(inputId, out string readableInputText);
            return ConvertInputConfigurationToAReadabledText(InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[inputId]) + " : " + readableInputText;
        }

        public static string ConvertInputToReadableText(string customDescription, InputID inputId)
        {
            return customDescription + " : " + ConvertInputConfigurationToAReadabledText(InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[inputId]);
        }

        public static string ConvertInputConfigurationToAReadabledText(InputConfigurationInherentData InputConfigurationInherentData,
            bool ShowPressType = true)
        {
            string returnString = String.Empty;
            /// Keys, Scroll or Mouse button names
            int inputCount = 0;

            foreach (var attributedKey in InputConfigurationInherentData.AttributedKeys)
            {
                if (inputCount > 0)
                {
                    returnString += " OR ";
                }

                returnString += GetInputControlName(InputControlLookup.Get().keyToKeyControlLookup[attributedKey]);
                inputCount += 1;
            }

            foreach (var mouseButton in InputConfigurationInherentData.AttributedMouseButtons)
            {
                if (inputCount > 0)
                {
                    returnString += " OR ";
                }

                returnString += GetInputControlName(InputControlLookup.Get().mouseButtonControlLookup[mouseButton]);
                inputCount += 1;
            }

            if (InputConfigurationInherentData.AttributedMouseScroll != MouseScroll.NONE)
            {
                if (inputCount > 0)
                {
                    returnString += " OR ";
                }

                returnString += GetInputControlName(InputControlLookup.Get().mouseScrollControlLookup[InputConfigurationInherentData.AttributedMouseScroll]);
                inputCount += 1;
            }

            if (ShowPressType)
            {
                if (InputConfigurationInherentData.DownHold)
                {
                    returnString += " Hold";
                }
                else if (InputConfigurationInherentData.Released)
                {
                    returnString += " Released";
                }
            }


            return returnString;
        }

        public static string GetInputControlName(InputControl inputControl)
        {
            if (!string.IsNullOrEmpty(inputControl.shortDisplayName) && inputControl.shortDisplayName != "Null")
            {
                return inputControl.shortDisplayName;
            }
            else if (!string.IsNullOrEmpty(inputControl.displayName) && inputControl.displayName != "Null")
            {
                return inputControl.displayName;
            }
            else if (!string.IsNullOrEmpty(inputControl.name) && inputControl.name != "Null")
            {
                return inputControl.name;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}