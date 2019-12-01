using CoreGame;
using Input;
using PlayerObject_Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Targetting
{
    /// <summary>
    /// Creates a <see cref="TargetCursor"/> UI object and handles it's movement (see <see cref="MoveCursor"/>)
    /// </summary>
    public class TargetCursorManager : GameSingleton<TargetCursorManager>
    {
        private TargettingConfiguration TargettingConfiguration;
        private IPlayerInteractiveObject PlayerInteractiveObjectRef;
        private GameInputManager GameInputManager;

        /// <summary>
        /// The <see cref="TargetCursor.transform.position"/> is in the range of the <see cref="CanvasScaler"/> target resolution.
        /// </summary>
        private GameObject TargetCursor;

        #region External Dependencies

        private TargettableInteractiveObjectScreenIntersectionManager TargettableInteractiveObjectScreenIntersectionManager = TargettableInteractiveObjectScreenIntersectionManager.Get();

        #endregion

        public TargetCursorManager()
        {
            this.TargettingConfiguration = TargettingConfigurationGameObject.Get().TargettingConfiguration;
            this.TargetCursor = GameObject.Instantiate(this.TargettingConfiguration.TargetCursorPrefab, CoreGameSingletonInstances.GameCanvas.transform);
            this.GameInputManager = GameInputManager.Get();
            this.InitializeTargetCursorPosition();
        }

        private void InitializeTargetCursorPosition()
        {
            this.TargetCursor.transform.position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

        public void Tick(float unscaled)
        {
            MoveCursor(unscaled);
            this.TargettableInteractiveObjectScreenIntersectionManager.Tick(unscaled, this.TargetCursor.transform.position);
        }

        private void MoveCursor(float d)
        {
            var CursorDisplacement = this.GameInputManager.CurrentInput.CursorDisplacement();
            var targetCursorDisplacementVector = (new Vector3(CursorDisplacement.x, CursorDisplacement.z) * d);
            var targetCursorPosition = this.TargetCursor.transform.position;
            this.TargetCursor.transform.position = new Vector3(Mathf.Clamp(targetCursorPosition.x + targetCursorDisplacementVector.x, 0f, Screen.width),
                Mathf.Clamp(targetCursorPosition.y + targetCursorDisplacementVector.y, 0f, Screen.height));
        }

        public Vector2 GetTargetCursorScreenPosition()
        {
            return this.TargetCursor.transform.position;
        }

        /// <summary>
        /// Returns a vector in the range of [-1,1] where [0,0] is the center.
        /// </summary>
        public Vector2 GetTargetCursorPositionAsDeltaFromCenter()
        {
            return (Camera.main.ScreenToViewportPoint(this.TargetCursor.transform.position) * 2).Add(new Vector3(-1f, -1f, -1f));
        }

#if UNITY_EDITOR
        /// <summary>
        /// Used for testing purpose.
        /// </summary>
        /// <param name="ScreenPosition">Pixel coordinates</param>
        public void SetTargetCursorPosition(Vector2 ScreenPosition)
        {
            this.TargetCursor.transform.position = ScreenPosition;
        }
#endif

        public override void OnDestroy()
        {
            if (this.TargetCursor != null)
            {
                GameObject.Destroy(this.TargetCursor.gameObject);
            }
        }
    }
}