﻿using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace Obstacle
{
    [Serializable]
    [SceneHandleDraw]
    public class ObstacleInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public ObstacleInteractiveObjectInitializerData InteractiveObjectInitializerData;

        public override CoreInteractiveObject Init()
        {
            return new ObstacleInteractiveObject(InteractiveGameObjectFactory.Build(this.gameObject), InteractiveObjectInitializerData);
        }
    }

    [Serializable]
    public class SquareObstacleSystemInitializationData
    {
        [Tooltip("Avoid tracking of value every frame. But obstacle frustum will never be updated")]
        public bool IsStatic = true;
    }

    [Serializable]
    [SceneHandleDraw]
    public class ObstacleInteractiveObjectInitializerData
    {
        [DrawNested] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectLogicCollider;
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData;
    }
}