using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

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
            this.InteractiveGameObject.CreateLogicCollider(ObstacleInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            ObstacleCollider = this.InteractiveGameObject.GetLogicColliderAsBox();
            SquareObstacleSystemInitializationData = ObstacleInteractiveObjectInitializerData.SquareObstacleSystemInitializationData;
            interactiveObjectTag = new InteractiveObjectTag {IsObstacle = 1};
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