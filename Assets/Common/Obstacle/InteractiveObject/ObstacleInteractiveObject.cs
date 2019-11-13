using System.Collections.Generic;
using CoreGame;
using GeometryIntersection;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Obstacle
{
    public class ObstacleInteractiveObject : CoreInteractiveObject
    {
        private ObstacleInteractiveObjectInitializerData ObstacleInteractiveObjectInitializerData;
        [VE_Nested] private SquareObstacleOcclusionFrustumsDefinition squareObstacleOcclusionFrustumsDefinition;

        public ObstacleInteractiveObject(IInteractiveGameObject interactiveGameObject, ObstacleInteractiveObjectInitializerData ObstacleInteractiveObjectInitializerData)
        {
            this.ObstacleInteractiveObjectInitializerData = ObstacleInteractiveObjectInitializerData;
            base.BaseInit(interactiveGameObject, false);
        }

        public override void Init()
        {
            this.InteractiveGameObject.CreateLogicCollider(ObstacleInteractiveObjectInitializerData.InteractiveObjectLogicCollider, LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES));
            this.CreatePhysicsCollider(ObstacleInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            this.CreateNavMeshObstacle(ObstacleInteractiveObjectInitializerData.SquareObstacleSystemInitializationData);
            ObstacleCollider = this.InteractiveGameObject.GetLogicColliderAsBox();
            SquareObstacleSystemInitializationData = ObstacleInteractiveObjectInitializerData.SquareObstacleSystemInitializationData;
            interactiveObjectTag = new InteractiveObjectTag {IsObstacle = true};
            squareObstacleOcclusionFrustumsDefinition = new SquareObstacleOcclusionFrustumsDefinition();

            ObstacleInteractiveObjectUniqueID = ObstacleInteractiveObjectManager.Get().OnSquareObstacleSystemCreated(this);
        }

        public int ObstacleInteractiveObjectUniqueID { get; private set; }
        public BoxCollider ObstacleCollider { get; private set; }
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData { get; private set; }

        public override void Destroy()
        {
            ObstacleInteractiveObjectManager.Get().OnSquareObstacleSystemDestroyed(this);
            base.Destroy();
        }

        private void CreatePhysicsCollider(InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition)
        {
            var physicsColliderObject = new GameObject("PhysicsCollider");
            physicsColliderObject.transform.parent = this.InteractiveGameObject.InteractiveGameObjectParent.transform;
            physicsColliderObject.transform.ResetLocal();
            var BoxCollider = physicsColliderObject.AddComponent<BoxCollider>();
            BoxCollider.center = InteractiveObjectBoxLogicColliderDefinition.LocalCenter;
            BoxCollider.size = InteractiveObjectBoxLogicColliderDefinition.LocalSize;
        }

        private void CreateNavMeshObstacle(SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData)
        {
            if (SquareObstacleSystemInitializationData.CreateNavMeshObstacle)
            {
                if (this.InteractiveGameObject.InteractiveGameObjectParent.GetComponent<NavMeshObstacle>() == null)
                {
                    var NavMeshObstacle = this.InteractiveGameObject.InteractiveGameObjectParent.AddComponent<NavMeshObstacle>();
                    NavMeshObstacle.shape = NavMeshObstacleShape.Box;
                    NavMeshObstacle.carving = true;
                }
            }
        }

        #region Data Retrieval

        /// <summary>
        ///     The center of the square obstacle is used by the occlusion frustums calculator.
        /// </summary>
        public TransformStruct GetObstacleCenterTransform()
        {
            return InteractiveGameObject.GetLogicColliderCenterTransform();
        }

        public List<FrustumV2> GetFaceFrustums()
        {
            return squareObstacleOcclusionFrustumsDefinition.FaceFrustums;
        }

        #endregion
    }
}