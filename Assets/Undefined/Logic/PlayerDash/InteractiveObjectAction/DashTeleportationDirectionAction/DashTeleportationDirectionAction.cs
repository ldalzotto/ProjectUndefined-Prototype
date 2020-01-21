using System.Collections.Generic;
using CoreGame;
using InteractiveObjectAction;
using InteractiveObjects;
using Targetting;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerDash
{
    public interface IEM_DashTeleportationDirectionAction_DataRetriever
    {
        Vector3 GetTargetWorldPosition();
    }

    public class DashTeleportationDirectionAction : AInteractiveObjectAction
    {
        public const string DashTeleportationDirectionActionUniqueID = "DashTeleportationDirectionAction";

        private DashPathCalculationSystem DashPathCalculationSystem;

        public DashTeleportationDirectionAction(CoreInteractiveObject associatedInteractiveObject,
            DashTeleportationDirectionActionDefinition DashTeleportationDirectionActionDefinition,
            CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition) : base(coreInteractiveObjectActionDefinition)
        {
            var targetCursormManagerRef = TargetCursorManager.Get();
            var mainCamera = Camera.main;
            this.DashPathCalculationSystem = new DashPathCalculationSystem(associatedInteractiveObject, DashTeleportationDirectionActionDefinition, targetCursormManagerRef, mainCamera);

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
        }

        public override void TickTimeFrozen(float d)
        {
            base.TickTimeFrozen(d);
            this.DashPathCalculationSystem.TickTimeFrozen(d);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.DashPathCalculationSystem.Dispose();
        }

        #region Data retrieval

        public Vector3 GetTargetWorldPosition()
        {
            return this.DashPathCalculationSystem.EndPoint;
        }

        #endregion
    }

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
            this.LineRenderer = new GameObject().AddComponent<LineRenderer>();
            this.LineRenderer.positionCount = 2;
        }

        private Vector3 StartPoint;
        private Vector3 DesiredEndPoint;
        public Vector3 EndPoint { get; private set; }

        public void Tick(float d)
        {
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

            if (Vector3.Distance(StartPoint, DesiredEndPoint) > DashTeleportationDirectionActionDefinition.MaxDashDistance)
            {
                this.EndPoint = StartPoint + ((DesiredEndPoint - StartPoint).normalized * DashTeleportationDirectionActionDefinition.MaxDashDistance);
            }
            else
            {
                this.EndPoint = DesiredEndPoint;
            }

            this.DebugLine();
        }

        public void TickTimeFrozen(float d)
        {
            this.Tick(d);
        }

        private LineRenderer LineRenderer;

        private void DebugLine()
        {
            this.LineRenderer.SetPosition(0, this.StartPoint);
            this.LineRenderer.SetPosition(1, this.EndPoint);
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