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
        private FiringProjectileSystem FiringProjectileSystem;
        private ExitActionSystem ExitActionSystem;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData, IPlayerInteractiveObject PlayerInteractiveObject) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition)
        {
            var gameInputManager = GameInputManager.Get();
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;
            this.TargetCursorSystem = new TargetCursorSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, gameInputManager);
            this.PlayerObjectOrientationSystem = new PlayerObjectOrientationSystem(this.FiringPlayerActionInherentData, PlayerInteractiveObject, this.TargetCursorSystem);
            this.FiringProjectileSystem = new FiringProjectileSystem(gameInputManager, FiringRecoilTimeManager.Get(), SpawnFiringProjectileEvent.Get(), FiringPlayerActionInherentData, PlayerInteractiveObject);
            this.ExitActionSystem = new ExitActionSystem(gameInputManager, this.TargetCursorSystem, this.PlayerObjectOrientationSystem);
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            this.TargetCursorSystem.CreateTargetCursor();
        }

        public override bool FinishedCondition()
        {
            return this.ExitActionSystem.ActionFinished;
        }

        public override void Tick(float d)
        {
            this.ExitActionSystem.Tick(d);
            if (!this.ExitActionSystem.ActionFinished)
            {
                this.TargetCursorSystem.Tick(d);
                this.PlayerObjectOrientationSystem.Tick(d);
                this.FiringProjectileSystem.Tick(d);
            }
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
            this.TargetCursor.transform.position = Camera.main.WorldToScreenPoint(
                playerTransform.position + (playerTransform.forward * this.FiringPlayerActionInherentDataRef.TargetCursorInitialOffset) // Eq (1)
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

        public void Dispose()
        {
            if (this.HorizontalPlaneGameObject != null)
            {
                GameObject.Destroy(this.HorizontalPlaneGameObject);
            }
        }
    }

    class FiringProjectileSystem
    {
        private GameInputManager GameInputManager;
        private FiringRecoilTimeManager FiringRecoilTimeManager;
        private SpawnFiringProjectileEvent SpawnFiringProjectileEvent;
        private FiringPlayerActionInherentData FiringPlayerActionInherentData;
        private IPlayerInteractiveObject IPlayerInteractiveObject;

        public FiringProjectileSystem(GameInputManager gameInputManager, FiringRecoilTimeManager firingRecoilTimeManager,
            SpawnFiringProjectileEvent spawnFiringProjectileEvent, FiringPlayerActionInherentData firingPlayerActionInherentData, IPlayerInteractiveObject IPlayerInteractiveObject)
        {
            GameInputManager = gameInputManager;
            FiringRecoilTimeManager = firingRecoilTimeManager;
            SpawnFiringProjectileEvent = spawnFiringProjectileEvent;
            FiringPlayerActionInherentData = firingPlayerActionInherentData;
            this.IPlayerInteractiveObject = IPlayerInteractiveObject;
        }

        public void Tick(float d)
        {
            if (this.GameInputManager.CurrentInput.GetInputCondition(InputID.FIRING_PROJECTILE_DOWN_HOLD) && this.FiringRecoilTimeManager.AuthorizeFiringAProjectile())
            {
                var FiringProjectileInitializerPrefab = this.FiringPlayerActionInherentData.FiringProjectileInitializerPrefab;
                var FiringProjectileInitializer = MonoBehaviour.Instantiate(FiringProjectileInitializerPrefab);
                FiringProjectileInitializer.Init();
                var FiredProjectile = FiringProjectileInitializer.GetCreatedFiredProjectile();
                var ProjectileSpawnLocalPosition = this.FiringPlayerActionInherentData.ProjectileSpawnLocalPosition;
                var FiredProjectileTransform = FiredProjectile.InteractiveGameObject.GetTransform();
                // Eq (2)
                FiredProjectile.InteractiveGameObject.InteractiveGameObjectParent.transform.position = this.IPlayerInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition + ProjectileSpawnLocalPosition;
                FiredProjectile.InteractiveGameObject.InteractiveGameObjectParent.transform.eulerAngles = this.IPlayerInteractiveObject.InteractiveGameObject.GetTransform().WorldRotationEuler;
                this.SpawnFiringProjectileEvent.OnFiringProjectileSpawned(this.FiringPlayerActionInherentData.RecoilTime);
            }
        }
    }

    class ExitActionSystem
    {
        public bool ActionFinished { get; private set; }
        private GameInputManager GameInputManager;
        private TargetCursorSystem TargetCursorSystem;
        private PlayerObjectOrientationSystem PlayerObjectOrientationSystem;

        public ExitActionSystem(GameInputManager gameInputManager, TargetCursorSystem targetCursorSystem, PlayerObjectOrientationSystem PlayerObjectOrientationSystem)
        {
            GameInputManager = gameInputManager;
            TargetCursorSystem = targetCursorSystem;
            this.PlayerObjectOrientationSystem = PlayerObjectOrientationSystem;
        }

        public void Tick(float d)
        {
            this.ActionFinished = this.GameInputManager.CurrentInput.FiringActionReleased();
            if (this.ActionFinished)
            {
                this.TargetCursorSystem.Dispose();
                this.PlayerObjectOrientationSystem.Dispose();
            }
        }
    }
}