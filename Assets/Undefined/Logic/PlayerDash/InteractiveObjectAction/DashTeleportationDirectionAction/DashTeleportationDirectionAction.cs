using CoreGame;
using InteractiveObjectAction;
using InteractiveObjects;
using Targetting;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerDash
{
    /// <summary>
    /// The <see cref="DashTeleportationDirectionAction"/> allow the Player to choose the target world position of teleportation by pointing the 
    /// cursor towards a direction.
    /// The effective teleportation is performed by <see cref="DashTeleportationAction"/>.
    /// </summary>
    public class DashTeleportationDirectionAction : AInteractiveObjectAction
    {
        public const string DashTeleportationDirectionActionUniqueID = "DashTeleportationDirectionAction";

        private DashPathCalculationSystem DashPathCalculationSystem;
        private DashPathVisualFeedbackSystem DashPathVisualFeedbackSystem;

        public DashTeleportationDirectionAction(CoreInteractiveObject associatedInteractiveObject,
            DashTeleportationDirectionActionDefinition DashTeleportationDirectionActionDefinition,
            CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition) : base(coreInteractiveObjectActionDefinition)
        {
            var targetCursormManagerRef = TargetCursorManager.Get();
            var mainCamera = Camera.main;
            this.DashPathCalculationSystem = new DashPathCalculationSystem(associatedInteractiveObject, DashTeleportationDirectionActionDefinition, targetCursormManagerRef, mainCamera);
            this.DashPathVisualFeedbackSystem = new DashPathVisualFeedbackSystem(new GameObject("DashTeleportationDirectionAction_VisualFeedback").AddComponent<LineRenderer>());

            this.Tick(0f);
        }

        public override string InteractiveObjectActionUniqueID
        {
            get { return DashTeleportationDirectionActionUniqueID; }
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.DashPathCalculationSystem.Tick(d);
            this.DashPathVisualFeedbackSystem.Tick(this.DashPathCalculationSystem.StartPoint, this.DashPathCalculationSystem.TargetWorldPosition);
        }

        public override void TickTimeFrozen(float d)
        {
            base.TickTimeFrozen(d);
            this.DashPathCalculationSystem.TickTimeFrozen(d);
            this.DashPathVisualFeedbackSystem.TickTimeFrozen(this.DashPathCalculationSystem.StartPoint, this.DashPathCalculationSystem.TargetWorldPosition);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.DashPathVisualFeedbackSystem.Dispose();
        }

        #region Data retrieval

        public Vector3? GetTargetWorldPosition()
        {
            return this.DashPathCalculationSystem.TargetWorldPosition;
        }

        #endregion
    }

    /// <summary>
    /// Responsible of : 
    ///     - Calculating the <see cref="TargetWorldPosition"/> from the target cursor position.
    /// </summary>
    struct DashPathCalculationSystem
    {
        private DashTeleportationDirectionActionDefinition DashTeleportationDirectionActionDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private TargetCursorManager TargetCursorManagerRef;
        private Camera MainCamera;

        public DashPathCalculationSystem(CoreInteractiveObject associatedInteractiveObject,
            DashTeleportationDirectionActionDefinition DashTeleportationDirectionActionDefinition, TargetCursorManager targetCursorManagerRef, Camera mainCamera) : this()
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.DashTeleportationDirectionActionDefinition = DashTeleportationDirectionActionDefinition;
            TargetCursorManagerRef = targetCursorManagerRef;
            MainCamera = mainCamera;
        }

        public Vector3 StartPoint { get; private set; }

        /// <summary>
        /// The <see cref="DesiredEndPoint"/> is the target world position obstructed by obstacles.
        /// It doesn't take into account contraints created by <see cref="DashTeleportationDirectionActionDefinition.MaxDashDistance"/>.
        /// /!\ This value can be null if it's calculation <see cref="CacluateDesiredEndPoint"/> cannot calculate it.
        /// </summary>
        private Vector3? DesiredEndPoint;

        /// <summary>
        /// The final target world position that will be used for external computations.
        /// /!\ This value can be null if it's calculation <see cref="CalculateTargetWorldPosition"/> cannot calculate it.
        /// </summary>
        public Vector3? TargetWorldPosition { get; private set; }

        public void Tick(float d)
        {
            this.CacluateDesiredEndPoint();
            this.CalculateTargetWorldPosition();
        }

        public void TickTimeFrozen(float d)
        {
            this.Tick(d);
        }

        /// <summary>
        /// Calculates the <see cref="DesiredEndPoint"/>.
        /// 
        /// 1/ Casting a PhysicsRay from the camera to <see cref="TargetCursorManagerRef"> and check if the ground is hitted.
        /// 2/ If the ground is hitted, then a NavMesh.Raycast is performed to take into account holes or obstacles.
        /// </summary>
        private void CacluateDesiredEndPoint()
        {
            this.DesiredEndPoint = null;
            var projectionRay = this.MainCamera.ScreenPointToRay(this.TargetCursorManagerRef.GetTargetCursorScreenPosition());
            this.StartPoint = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
            if (Physics.Raycast(projectionRay, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER)))
            {
                var pointedGroundPosition = hit.point;

                NavMesh.Raycast(AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition, pointedGroundPosition, out NavMeshHit navMeshRaycastHit, 1 << NavMesh.GetAreaFromName(NavMeshConstants.WALKABLE_LAYER));
                if (navMeshRaycastHit.distance > 0f)
                {
                    this.DesiredEndPoint = navMeshRaycastHit.position;
                }
            }
        }


        /// <summary>
        /// Calculates the <see cref="TargetWorldPosition"/>.
        /// This calculation is done by constrainning the <see cref="DesiredEndPoint"/> by the max distance allowed by <see cref="DashTeleportationDirectionActionDefinition.MaxDashDistance"/>.
        /// </summary>
        private void CalculateTargetWorldPosition()
        {
            this.TargetWorldPosition = null;
            if (this.DesiredEndPoint.HasValue)
            {
                if (Vector3.Distance(StartPoint, DesiredEndPoint.Value) > DashTeleportationDirectionActionDefinition.MaxDashDistance)
                {
                    this.TargetWorldPosition = StartPoint + ((DesiredEndPoint.Value - StartPoint).normalized * DashTeleportationDirectionActionDefinition.MaxDashDistance);
                }
                else
                {
                    this.TargetWorldPosition = DesiredEndPoint;
                }
            }

        }

    }

    /// <summary>
    /// Responsible of :
    ///     - Creatign a line indicating the dash teleportation path.
    /// </summary>
    struct DashPathVisualFeedbackSystem
    {
        public DashPathVisualFeedbackSystem(LineRenderer InstanciatedLineRenderer)
        {
            this.LineRenderer = InstanciatedLineRenderer;
        }

        private LineRenderer LineRenderer;

        /// <summary>
        /// /!\ This method must be called after <see cref="DashPathCalculationSystem.Tick(float)"/> because input datas are recovered from <see cref="DashPathCalculationSystem"/>.
        /// </summary>
        public void Tick(Vector3 StartPoint, Vector3? TargetWorldPosition)
        {
            if (TargetWorldPosition.HasValue)
            {
                this.LineRenderer.positionCount = 2;
                this.LineRenderer.SetPosition(0, StartPoint);
                this.LineRenderer.SetPosition(1, TargetWorldPosition.Value);
            }
            else
            {
                this.LineRenderer.positionCount = 0;
            }
        }

        public void TickTimeFrozen(Vector3 StartPoint, Vector3? TargetWorldPosition)
        {
            this.Tick(StartPoint, TargetWorldPosition);
        }

        public void Dispose()
        {
            if (this.LineRenderer != null)
            {
                GameObject.Destroy(this.LineRenderer.gameObject);
            }
        }
    }
}