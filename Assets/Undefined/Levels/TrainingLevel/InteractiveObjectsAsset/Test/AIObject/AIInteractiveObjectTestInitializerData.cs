using System;
using AIObjects;
using InteractiveObjects_Interfaces;
using RTPuzzle;
using UnityEngine;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "AIInteractiveObjectTestInitializerData", menuName = "Test/AIInteractiveObjectTestInitializerData", order = 1)]
    public class AIInteractiveObjectTestInitializerData : AbstractAIInteractiveObjectInitializerData
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIPatrolSystemDefinition AIPatrolSystemDefinition;

        [DrawNested] [Inline(CreateAtSameLevelIfAbsent = true)]
        public SightObjectSystemDefinition SightObjectSystemDefinition;

        public AIMovementSpeedDefinition AISpeedWhenAttracted;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public LocalPuzzleCutsceneTemplate DisarmObjectAnimation;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new AIInteractiveObjectTest(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}