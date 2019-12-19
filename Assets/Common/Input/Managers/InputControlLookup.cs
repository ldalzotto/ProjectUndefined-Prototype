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
    }
}