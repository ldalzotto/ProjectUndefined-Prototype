using CoreGame;
using Input;
using TMPro;
using UnityEngine;

namespace InputDynamicTextMenu
{
    public abstract class AInputTextModule
    {
        protected TextMeshProUGUI InstanciatedText;

        protected AInputTextModule(RectTransform ParentTransform)
        {
            var instaciatedText = MonoBehaviour.Instantiate(InputDynamicTextMenuConfigurationGameObject.Get().InputDynamicTextMenuConfiguration.InputDynamicTextMenuModuleTemplatePrefab, ParentTransform);
            this.InstanciatedText = instaciatedText.GetComponent<TextMeshProUGUI>();
            (this.InstanciatedText.transform as RectTransform).ResetLocalPositionAndRotation();
        }

        public void Enable()
        {
            this.InstanciatedText.gameObject.SetActive(true);
        }

        public void Disable()
        {
            this.InstanciatedText.gameObject.SetActive(false);
        }
    }

    public class LocomotionTextModule : AInputTextModule
    {
        public LocomotionTextModule(RectTransform ParentTransform) : base(ParentTransform)
        {
            this.InstanciatedText.text =
                InputConfigurationInherentData2ReadableText.ConvertInputConfigurationToAReadabledText(InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[InputID.UP_DOWN_HOLD], ShowPressType: false)
                + ", " + InputConfigurationInherentData2ReadableText.ConvertInputConfigurationToAReadabledText(InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[InputID.LEFT_DOWN_HOLD], ShowPressType: false)
                + ", " + InputConfigurationInherentData2ReadableText.ConvertInputConfigurationToAReadabledText(InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[InputID.RIGHT_DOWN_HOLD], ShowPressType: false)
                + ", " + InputConfigurationInherentData2ReadableText.ConvertInputConfigurationToAReadabledText(InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[InputID.DOWN_DOWN_HOLD], ShowPressType: false)
                + " : Movement.";
        }
    }

    public class CameraTextModule : AInputTextModule
    {
        public CameraTextModule(RectTransform ParentTransform) : base(ParentTransform)
        {
            this.InstanciatedText.text = InputConfigurationInherentData2ReadableText.ConvertInputConfigurationToAReadabledText(InputConfigurationGameObject.Get().InputConfiguration.ConfigurationInherentData[InputID.CAMERA_ROTATION_DOWN_HOLD]) + " + Mouse movement : Camera rotation." + "\r\n" +
                                         InputConfigurationInherentData2ReadableText.ConvertInputToReadableText(InputID.CAMERA_ZOOM);
        }
    }

    public class FiringModeEnterTextModule : AInputTextModule
    {
        public FiringModeEnterTextModule(RectTransform ParentTransform) : base(ParentTransform)
        {
            this.InstanciatedText.text = InputConfigurationInherentData2ReadableText.ConvertInputToReadableText(InputID.FIRING_ACTION_DOWN);
        }
    }

    public class OnTargettingTextModule : AInputTextModule
    {
        public OnTargettingTextModule(RectTransform ParentTransform) : base(ParentTransform)
        {
            this.InstanciatedText.text = InputConfigurationInherentData2ReadableText.ConvertInputToReadableText(InputID.FIRING_PROJECTILE_DOWN_HOLD);
        }
    }

    public class DelflectionTextModule : AInputTextModule
    {
        public DelflectionTextModule(RectTransform ParentTransform) : base(ParentTransform)
        {
            this.InstanciatedText.text = InputConfigurationInherentData2ReadableText.ConvertInputToReadableText(InputID.DEFLECT_PROJECTILE_DOWN);
        }
    }
}