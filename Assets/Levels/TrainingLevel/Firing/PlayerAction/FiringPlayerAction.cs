using CoreGame;
using Input;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;

namespace Firing
{
    public class FiringPlayerAction : PlayerAction
    {
        private FiringPlayerActionInherentData FiringPlayerActionInherentData;

        private TargetCursorSystem TargetCursorSystem;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData, IPlayerInteractiveObject PlayerInteractiveObject) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition)
        {
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;
            this.TargetCursorSystem = new TargetCursorSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, GameInputManager.Get());
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, this.TargetCursorSystem);
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            this.TargetCursorSystem.CreateTargetCursor();
        }

        public override bool FinishedCondition()
        {
            return false;
        }

        public override void Tick(float d)
        {
            this.TargetCursorSystem.Tick(d);
            this.PlayerObjectOrientationSystem.Tick(d);
        }

        public override void LateTick(float d)
        {
        }

        public override void GUITick()
        {
        }

        public override void GizmoTick()
        {
        }
    }

    class TargetCursorSystem
    {
        private FiringPlayerActionInherentData FiringPlayerActionInherentDataRef;
        private IPlayerInteractiveObject PlayerInteractiveObjectRef;
        private GameInputManager GameInputManager;
        private GameObject TargetCursor;

        public TargetCursorSystem(FiringPlayerActionInherentData firingPlayerActionInherentDataRef, IPlayerInteractiveObject PlayerInteractiveObjectRef, GameInputManager GameInputManager)
        {
            FiringPlayerActionInherentDataRef = firingPlayerActionInherentDataRef;
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.TargetCursor = null;
            this.GameInputManager = GameInputManager;
        }

        public void CreateTargetCursor()
        {
            this.TargetCursor = GameObject.Instantiate(this.FiringPlayerActionInherentDataRef.TargetCursorPrefab, CoreGameSingletonInstances.GameCanvas.transform);
            var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
            this.TargetCursor.transform.position = Camera.main.WorldToScreenPoint(playerTransform.position + (playerTransform.forward * this.FiringPlayerActionInherentDataRef.TargetCursorInitialOffset));
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
    }

    class PlayerObjectOrientationSystem
    {
        private IPlayerInteractiveObject PlayerInteractiveObjectRef;
        private GameObject HorizontalPlaneGameObject;
        private TargetCursorSystem TargetCursorSystemRef;

        public PlayerObjectOrientationSystem(FiringPlayerActionInherentData firingPlayerActionInherentDataRef, IPlayerInteractiveObject PlayerInteractiveObjectRef, TargetCursorSystem TargetCursorSystemRef)
        {
            this.HorizontalPlaneGameObject = GameObject.Instantiate(firingPlayerActionInherentDataRef.FiringHorizontalPlanePrefab);
            this.HorizontalPlaneGameObject.layer = LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER);
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.TargetCursorSystemRef = TargetCursorSystemRef;
        }

        public void Tick(float d)
        {
            this.HorizontalPlaneGameObject.transform.position = this.PlayerInteractiveObjectRef.InteractiveGameObject.GetTransform().WorldPosition;
            var projectionRay = Camera.main.ScreenPointToRay(this.TargetCursorSystemRef.GetTargetCursorScreenPosition());
            if (Physics.Raycast(projectionRay, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerConstants.FIRING_ACTION_HORIZONTAL_LAYER)))
            {
                var projectedPosition = hit.point;
                var playerTransform = this.PlayerInteractiveObjectRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
                var lookDirection = (projectedPosition - playerTransform.position).normalized;

                var rotationAngle = Vector3.SignedAngle(Vector3.forward, lookDirection, playerTransform.up);
                playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, rotationAngle, playerTransform.eulerAngles.z);
            }
        }
    }
}