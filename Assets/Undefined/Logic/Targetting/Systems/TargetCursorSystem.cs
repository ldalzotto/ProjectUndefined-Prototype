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
    public class TargetCursorSystem
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
        private TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager = TargettableInteractiveObjectSelectionManager.Get();

        #endregion

        public TargetCursorSystem(IPlayerInteractiveObject PlayerInteractiveObjectRef, GameInputManager GameInputManager)
        {
            this.TargettingConfiguration = TargettingConfigurationGameObject.Get().TargettingConfiguration;
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.TargetCursor = null;
            this.GameInputManager = GameInputManager;
        }

        public void CreateTargetCursor()
        {
            this.TargetCursor = GameObject.Instantiate(this.TargettingConfiguration.TargetCursorPrefab, CoreGameSingletonInstances.GameCanvas.transform);
            var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
            OffsetTargetCursorPositionAtStart(playerTransform);
        }

        private void OffsetTargetCursorPositionAtStart(Transform playerTransform)
        {
            this.TargetCursor.transform.position = Camera.main.WorldToScreenPoint(
                playerTransform.position + (playerTransform.forward * this.TargettingConfiguration.TargetCursorInitialOffset) // Eq (1)
            );
        }

        public void Tick(float d)
        {
            MoveCursor(d);
            this.TargettableInteractiveObjectScreenIntersectionManager.Tick(d, this.TargetCursor.transform.position);
        }

        /// <summary>
        /// Moves the <see cref="TargetCursor"/> either from <see cref="GameInputManager"/> or if an actual object is targetted
        /// <see cref="TargettableInteractiveObjectSelectionManager.CurrentlyTargettedInteractiveObject"/> then setted to it's center
        /// </summary>
        private void MoveCursor(float d)
        {
            if (this.TargettableInteractiveObjectSelectionManager.CurrentlyTargettedInteractiveObject != null)
            {
                this.TargetCursor.transform.position = Camera.main.WorldToScreenPoint(TargettableInteractiveObjectSelectionManager.CurrentlyTargettedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().center);
            }
            else
            {
                var CursorDisplacement = this.GameInputManager.CurrentInput.CursorDisplacement();
                this.TargetCursor.transform.position += (new Vector3(CursorDisplacement.x, CursorDisplacement.z) * d);
            }
        }

        public Vector2 GetTargetCursorScreenPosition()
        {
            return this.TargetCursor.transform.position;
        }

        public void Dispose()
        {
            if (this.TargetCursor != null)
            {
                GameObject.Destroy(this.TargetCursor.gameObject);
            }
        }
    }
}