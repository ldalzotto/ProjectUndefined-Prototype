using System;
using CoreGame;
using Input;
using TMPro;
using UnityEngine;

namespace ProjectileDeflection
{
    public class ProjectileDeflectionFeedbackIcon
    {
        private GameObject InstanciatedGameObject;
        private RectTransform Transform;
        private TMPro.TextMeshProUGUI inputText;

        private Camera mainCamera;

        public ProjectileDeflectionFeedbackIcon(GameObject instanciatedGameObject, InputID DisplayedInput)
        {
            InstanciatedGameObject = instanciatedGameObject;
            this.Transform = this.InstanciatedGameObject.transform as RectTransform;
            this.mainCamera = Camera.main;
            this.inputText = this.Transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            var feedbackInputControl = InputControlLookup.FindTheFirstInputControlForInputID(DisplayedInput);
            if (feedbackInputControl != null)
            {
                this.inputText.text = InputControlLookup.GetInputControlRawName(feedbackInputControl);
            }
        }

        public void SetPositionFromWorld(Vector3 worldPosition)
        {
            this.Transform.position = this.mainCamera.WorldToScreenPoint(worldPosition);
        }

        public void Destroy()
        {
            if (this.InstanciatedGameObject != null)
            {
                GameObject.Destroy(this.InstanciatedGameObject);
            }
        }


        public static ProjectileDeflectionFeedbackIcon Build(GameObject ProjectileDeflectionFeedbackPrefab, InputID DisplayedInput)
        {
            return new ProjectileDeflectionFeedbackIcon(MonoBehaviour.Instantiate(ProjectileDeflectionFeedbackPrefab, ProjectileDeflectionFeedbackIconContainer.Get().transform), DisplayedInput);
        }
    }

    public class ProjectileDeflectionFeedbackIconContainer : MonoBehaviour
    {
        private static ProjectileDeflectionFeedbackIconContainer Instance;

        public static ProjectileDeflectionFeedbackIconContainer Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<ProjectileDeflectionFeedbackIconContainer>();
                if (Instance == null)
                {
                    var obj = new GameObject("ProjectileDeflectionFeedbackIconContainer", new Type[] {typeof(RectTransform)});
                    obj.transform.parent = CoreGameSingletonInstances.GameCanvas.transform;
                    (obj.transform as RectTransform).ResetLocalPositionAndRotation();
                    Instance = obj.AddComponent<ProjectileDeflectionFeedbackIconContainer>();
                }
            }

            return Instance;
        }
    }
}