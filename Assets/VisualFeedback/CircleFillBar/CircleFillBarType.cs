using UnityEngine;

namespace VisualFeedback
{
    public class CircleFillBarType : MonoBehaviour
    {
        #region External Dependencies

        private CircleFillBarRendererManager CircleFillBarRendererManager = CircleFillBarRendererManager.Get();

        #endregion

        private Camera cam;
        private float currentProgression;

        public float CurrentProgression
        {
            get => currentProgression;
        }

        public void Init(Camera camera)
        {
            this.cam = camera;
            this.CircleFillBarRendererManager.OnCircleFillBarTypeCreated(this);
        }

        public void Tick(float progression)
        {
            this.currentProgression = progression;
            this.transform.LookAt(this.cam.transform);
        }

        public void OnCircleFillBarTypeEnabled()
        {
            this.CircleFillBarRendererManager.OnCircleFillBarTypeCreated(this);
        }

        private void OnDisable()
        {
            if (this.CircleFillBarRendererManager != null)
            {
                this.CircleFillBarRendererManager.OnCircleFillBarTypeDestroyed(this);
            }
        }

        public static void EnableInstace(CircleFillBarType CircleFillBarTypeRef)
        {
            CircleFillBarTypeRef.gameObject.SetActive(true);
            CircleFillBarTypeRef.OnCircleFillBarTypeEnabled();
        }
    }
}