using CoreGame;
using Input;
using PlayerObject_Interfaces;
using UnityEngine;

namespace Targetting
{
    public class TargetCursorSystem
    {
        private TargettingConfiguration TargettingConfiguration;
        private IPlayerInteractiveObject PlayerInteractiveObjectRef;
        private GameInputManager GameInputManager;
        private GameObject TargetCursor;

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
            var CursorDisplacement = this.GameInputManager.CurrentInput.CursorDisplacement();
            this.TargetCursor.transform.position += (new Vector3(CursorDisplacement.x, CursorDisplacement.z) * d);
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