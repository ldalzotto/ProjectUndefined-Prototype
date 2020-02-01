using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace Obstacle
{
    [Serializable]
    [SceneHandleDraw]
    public class ObstacleInteractiveObjectDefinition : AbstractInteractiveObjectV2Definition
    {
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData;
        [DrawNested] [Inline()] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectLogicCollider;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new ObstacleInteractiveObject(InteractiveGameObjectFactory.Build_Allocate(interactiveGameObject), this);
        }
    }


    [Serializable]
    public class SquareObstacleSystemInitializationData
    {
        [Tooltip("Avoid tracking of value every frame. But obstacle frustum will never be updated")]
        public bool IsStatic = true;

        public bool CreateNavMeshObstacle;
    }
}