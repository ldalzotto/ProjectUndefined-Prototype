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
        private ObstacleInteractiveObjectDefinition _obstacleInteractiveObjectDefinition;
        [VE_Nested] private SquareObstacleOcclusionFrustumsDefinition squareObstacleOcclusionFrustumsDefinition;

        public ObstacleInteractiveObject(IInteractiveGameObject interactiveGameObject, ObstacleInteractiveObjectDefinition ObstacleInteractiveObjectDefinition)
        {
            this._obstacleInteractiveObjectDefinition = ObstacleInteractiveObjectDefinition;
            base.BaseInit(interactiveGameObject, false);
        }

        public override void Init()
        {
            this.InteractiveGameObject.CreateLogicCollider(_obstacleInteractiveObjectDefinition.InteractiveObjectLogicCollider, LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES));
            this.CreatePhysicsCollider(_obstacleInteractiveObjectDefinition.InteractiveObjectLogicCollider);
            this.CreateNavMeshObstacle(_obstacleInteractiveObjectDefinition.SquareObstacleSystemInitializationData, _obstacleInteractiveObjectDefinition.InteractiveObjectLogicCollider);
            ObstacleCollider = this.InteractiveGameObject.GetLogicColliderAsBox();
            SquareObstacleSystemInitializationData = _obstacleInteractiveObjectDefinition.SquareObstacleSystemInitializationData;
            interactiveObjectTag = new InteractiveObjectTag {IsObstacle = true};
            squareObstacleOcclusionFrustumsDefinition = new SquareObstacleOcclusionFrustumsDefinition(_obstacleInteractiveObjectDefinition.InteractiveObjectLogicCollider);

            ObstacleInteractiveObjectUniqueID = ObstacleInteractiveObjectManager.Get().OnSquareObstacleSystemCreated(this);
        }

        public int ObstacleInteractiveObjectUniqueID { get; private set; }
        public BoxCollider ObstacleCollider { get; private set; }
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData { get; private set; }

        public override void Destroy()
        {
            ObstacleOcclusionCalculationManagerV2.Get().OnObstacleInteractiveObjectDestroyed(this);
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

        private void CreateNavMeshObstacle(SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData, InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition)
        {
            if (SquareObstacleSystemInitializationData.CreateNavMeshObstacle)
            {
                if (this.InteractiveGameObject.InteractiveGameObjectParent.GetComponent<NavMeshObstacle>() == null)
                {
                    var NavMeshObstacle = this.InteractiveGameObject.InteractiveGameObjectParent.AddComponent<NavMeshObstacle>();
                    NavMeshObstacle.shape = NavMeshObstacleShape.Box;
                    NavMeshObstacle.carving = true;
                    NavMeshObstacle.center = InteractiveObjectBoxLogicColliderDefinition.LocalCenter;
                    NavMeshObstacle.size = InteractiveObjectBoxLogicColliderDefinition.LocalSize;
                }
            }
        }

        #region Data Retrieval

        public List<FrustumV2> GetFaceFrustums()
        {
            return squareObstacleOcclusionFrustumsDefinition.FaceFrustums;
        }

        #endregion
    }
}