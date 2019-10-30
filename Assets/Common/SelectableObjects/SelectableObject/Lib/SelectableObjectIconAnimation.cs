using UnityEngine;

namespace SelectableObject
{
    internal class SelectableObjectIconAnimation
    {
        private const float MaxIconScale = 1f;
        private const float MinIconScale = 0.5f;

        private float iconScale;
        private float rotationAngle;

        public SelectableObjectIconAnimation()
        {
            iconScale = MaxIconScale;
        }

        public float GetIconScale()
        {
            return Mathf.Clamp(iconScale, MinIconScale, MaxIconScale);
        }

        public float GetRotationAngleDeg()
        {
            return rotationAngle;
        }

        public void Tick(float d, bool hasMultipleAvailableSelectionObjects)
        {
            iconScale -= d * 2f;
            if (iconScale <= 0) iconScale = MaxIconScale;

            if (hasMultipleAvailableSelectionObjects)
                //   this.iconScale = MaxIconScale;
                rotationAngle += d * 360f;
            else
                rotationAngle = 0f;
        }
    }
}