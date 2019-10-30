using System;
using OdinSerializer;
using UnityEngine;

namespace Input
{
    [Serializable]
    [CreateAssetMenu(fileName = "CoreInputConfiguration", menuName = "Configuration/CoreGame/StaticConfiguration/CoreInputConfiguration", order = 1)]
    public class CoreInputConfiguration : SerializedScriptableObject
    {
        public float MouseSensitivity = 1f;
        public float CameraMovementFactor = 1f;
        public float CursorMovementFactor = 1f;

        #region Data Retrieval

        public float GetCameraMovementMouseSensitivity()
        {
            return this.MouseSensitivity * this.CameraMovementFactor;
        }

        public float GetCursorMovementMouseSensitivity()
        {
            return this.MouseSensitivity * this.CursorMovementFactor;
        }

        #endregion
    }
}