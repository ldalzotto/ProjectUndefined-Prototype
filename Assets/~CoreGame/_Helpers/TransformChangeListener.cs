using System;
using UnityEngine;

namespace CoreGame
{
    public class BlittableTransformChangeListenerManager
    {
        private bool positionListening;
        private bool rotationListening;
        private TransformChangeListener TransformChangeListener;

        private Nullable<Vector3> lastFramePosition;
        private Nullable<Vector3> lastFrameRotation;

        private bool positionChangedThatFrame;
        private bool rotationChangedThatFrame;

        public BlittableTransformChangeListenerManager(bool positionListening, bool rotationListening, TransformChangeListener TransformChangeListener = null)
        {
            this.positionListening = positionListening;
            this.rotationListening = rotationListening;
            this.TransformChangeListener = TransformChangeListener;
            this.positionChangedThatFrame = false;
            this.rotationChangedThatFrame = false;
        }

        #region Logical Conditions

        public bool TransformChangedThatFrame()
        {
            return this.positionChangedThatFrame || this.rotationChangedThatFrame;
        }

        #endregion

        public void Tick(Vector3 worldPosition, Vector3 worldRotationEuler)
        {
            this.positionChangedThatFrame = false;
            this.rotationChangedThatFrame = false;

            if (this.positionListening)
            {
                if (this.lastFramePosition == null)
                {
                    this.PositionChanged();
                }
                else
                {
                    if (this.lastFramePosition.Value != worldPosition)
                    {
                        this.PositionChanged();
                    }
                }

                this.lastFramePosition = worldPosition;
            }

            if (this.rotationListening)
            {
                if (this.lastFrameRotation == null)
                {
                    this.RotationChanged();
                }
                else
                {
                    if (this.lastFrameRotation.Value != worldRotationEuler)
                    {
                        this.RotationChanged();
                    }
                }

                this.lastFrameRotation = worldRotationEuler;
            }
        }

        private void PositionChanged()
        {
            if (this.TransformChangeListener != null)
            {
                this.TransformChangeListener.onPositionChange();
            }

            this.positionChangedThatFrame = true;
        }

        private void RotationChanged()
        {
            if (this.TransformChangeListener != null)
            {
                this.TransformChangeListener.onRotationChange();
            }

            this.rotationChangedThatFrame = true;
        }
    }

    public interface TransformChangeListener
    {
        void onPositionChange();
        void onRotationChange();
    }
}