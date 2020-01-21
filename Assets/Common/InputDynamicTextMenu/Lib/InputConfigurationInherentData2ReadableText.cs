using System;
using System.Collections.Generic;
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
            {InputID.FIRING_ACTION_DOWN, "Entering firing mode"},
            {InputID.FIRING_PROJECTILE_DOWN_HOLD, "Firing"},
            {InputID.FREEZE_TIME_DOWN, "Freeze time"},
            {InputID.CANCEL_DOWN, "Cancel action" }
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
                    returnString += "<b><color=\"yellow\">" + " Hold" + "</color></b>";
                }
                else if (InputConfigurationInherentData.Released)
                {
                    returnString += "<b><color=\"yellow\">" + " Released" + "</color></b>";
                }
            }


            return returnString;
        }

        public static string GetInputControlName(InputControl inputControl)
        {
            return "<b><color=\"red\">" + InputControlLookup.GetInputControlRawName(inputControl) + "</color></b>";
        }
    }
}