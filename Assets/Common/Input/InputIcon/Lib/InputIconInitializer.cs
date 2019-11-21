using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Input
{
    public class InputIconInitializer : MonoBehaviour
    {
        [CustomEnum()] public InputID InputID;
        private bool OneInputIconAlreadyCreated;

        public void Build()
        {
            var ConfigurationInherentData = InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[this.InputID];

            if (ConfigurationInherentData.AttributedKeys != null)
            {
                foreach (var attributedKey in ConfigurationInherentData.AttributedKeys)
                {
                    CreateKeyImage(ConfigurationInherentData, attributedKey);
                }
            }

            if (ConfigurationInherentData.AttributedMouseButtons != null)
            {
                foreach (var attributedMouseButton in ConfigurationInherentData.AttributedMouseButtons)
                {
                    CeateMouseButtonImage(ConfigurationInherentData, attributedMouseButton);
                }
            }

            GameObject.Destroy(this.gameObject);
        }


        private void CreateKeyImage(InputConfigurationInherentData ConfigurationInherentData, Key key)
        {
            this.CreateOrTextIfNecessary();
            var keyImage = MonoBehaviour.Instantiate(InputConfigurationGameObject.Get().CoreInputConfiguration.KeyIconPrefab, this.transform.parent);
            keyImage.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
            var keyText = keyImage.GetComponentInChildren<Text>();
            keyText.text = GameInputManager.Get().GetKeyToKeyControlLookup()[key].displayName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(keyImage.transform as RectTransform);
            (keyImage.transform as RectTransform).sizeDelta = new Vector2(LayoutUtility.GetPreferredHeight(keyImage.transform as RectTransform), LayoutUtility.GetPreferredHeight(keyImage.transform as RectTransform));
            this.OneInputIconAlreadyCreated = true;
        }

        
        private void CeateMouseButtonImage(InputConfigurationInherentData configurationInherentData, MouseButton attributedMouseButton)
        {
            this.CreateOrTextIfNecessary();
            Image imageprefab = null;
            switch (attributedMouseButton)
            {
                case MouseButton.LEFT_BUTTON:
                    imageprefab = InputConfigurationGameObject.Get().CoreInputConfiguration.LeftClickIconPrefab;
                    break;
                case MouseButton.RIGHT_BUTTON:
                    imageprefab = InputConfigurationGameObject.Get().CoreInputConfiguration.RightClickIconPrefab;
                    break;
            }
            var keyImage = MonoBehaviour.Instantiate(imageprefab, this.transform.parent);
            keyImage.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);

            this.OneInputIconAlreadyCreated = true;
        }

        private void CreateOrTextIfNecessary()
        {
            if (this.OneInputIconAlreadyCreated)
            {
                var orTextObject = new GameObject("OR", typeof(RectTransform));
                orTextObject.transform.parent = this.transform.parent;
                orTextObject.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
                var text = orTextObject.AddComponent<Text>();
                text.text = " or ";
            }
        }
    }
}