namespace CoreGame
{
    /*
    public class InputImageType : MonoBehaviour
    {
        private InputImageTypeInstanceType inputImageTypeInstanceType;

        public static InputImageType Instantiate(InputConfigurationInherentData InputConfigurationInherentData, Transform parent = null, bool animate = false)
        {
            var corePrefabConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CorePrefabConfiguration;

            InputImageType InputImageType = null;
            InputImageType prefabToInstanciate = null;

            var InputImageTypeInstanceType = GetInputImageType(InputConfigurationInherentData);

            if (InputImageTypeInstanceType == InputImageTypeInstanceType.KEY)
            {
                prefabToInstanciate = corePrefabConfiguration.InputBaseImage;
            }
            else if (InputImageTypeInstanceType == InputImageTypeInstanceType.LEFT_MOUSE)
            {
                prefabToInstanciate = corePrefabConfiguration.LeftMouseBaseImage;
            }
            else if (InputImageTypeInstanceType == InputImageTypeInstanceType.RIGHT_MOUSE)
            {
                prefabToInstanciate = corePrefabConfiguration.RightMouseBaseImage;
            }

            if (parent != null)
            {
                InputImageType = Instantiate(prefabToInstanciate, parent);
            }
            else
            {
                InputImageType = Instantiate(prefabToInstanciate);
            }

            InputImageType.Init(InputImageTypeInstanceType, InputConfigurationInherentData, animate);
            return InputImageType;
        }

        #region Internal Dependencies

        private Text KeyText;

        #endregion

        public InputImageTypeInstanceType InputImageTypeInstanceType
        {
            get => inputImageTypeInstanceType;
        }

        public void Init(InputImageTypeInstanceType InputImageTypeInstanceType, InputConfigurationInherentData InputConfigurationInherentData, bool animate)
        {
            this.inputImageTypeInstanceType = InputImageTypeInstanceType;
            this.KeyText = GetComponentInChildren<Text>();
            if (!animate)
            {
                GetComponent<Animator>().enabled = false;
            }

            CoreGameSingletonInstances.GameInputManager.GetKeyToKeyControlLookup().TryGetValue(InputConfigurationInherentData.AttributedKeys[0], out KeyControl retrievedKeyControl);
            if (retrievedKeyControl != null)
            {
                this.SetKey(retrievedKeyControl.displayName);
            }
        }

        public void SetKey(string key)
        {
            if (this.inputImageTypeInstanceType == InputImageTypeInstanceType.KEY)
            {
                this.KeyText.text = key;
            }
        }

        public void SetTextFontSize(int fontSize)
        {
            if (this.inputImageTypeInstanceType == InputImageTypeInstanceType.KEY)
            {
                this.KeyText.fontSize = fontSize;
            }
        }

        public static InputImageTypeInstanceType GetInputImageType(InputConfigurationInherentData inputConfigurationInherentData)
        {
            InputImageTypeInstanceType InputImageTypeInstanceType = InputImageTypeInstanceType.NONE;
            var keyAttributedButton = inputConfigurationInherentData.GetAssociatedInputKey();
            if (keyAttributedButton != Key.None)
            {
                InputImageTypeInstanceType = InputImageTypeInstanceType.KEY;
            }
            else
            {
                var mouseAttributedButton = inputConfigurationInherentData.GetAssociatedMouseButton();
                if (mouseAttributedButton != MouseButton.NONE)
                {
                    if (mouseAttributedButton == MouseButton.LEFT_BUTTON)
                    {
                        InputImageTypeInstanceType = InputImageTypeInstanceType.LEFT_MOUSE;
                    }
                    else if (mouseAttributedButton == MouseButton.RIGHT_BUTTON)
                    {
                        InputImageTypeInstanceType = InputImageTypeInstanceType.RIGHT_MOUSE;
                    }
                }
                else
                {
                    var scrollAttributedButton = inputConfigurationInherentData.GetAssociatedMouseScroll();
                    if (scrollAttributedButton != MouseScroll.NONE)
                    {
                        InputImageTypeInstanceType = InputImageTypeInstanceType.SCROLL;
                    }
                }
            }

            return InputImageTypeInstanceType;
        }
    }

    public enum InputImageTypeInstanceType
    {
        NONE = 0,
        KEY = 1,
        LEFT_MOUSE = 2,
        RIGHT_MOUSE = 3,
        SCROLL = 4
    }
    */
}