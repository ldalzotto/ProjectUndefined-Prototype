using System;
using InteractiveObjects_Interfaces;
using RTPuzzle;
using SelectableObject;
using UnityEngine;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "TestAttractiveObjectInitializerData", menuName = "Test/TestAttractiveObjectInitializerData", order = 1)]
    public class TestAttractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public InteractiveObjectLogicColliderDefinition InteractiveObjectLogicCollider;

        [DrawNested] [Inline(createAtSameLevelIfAbsent: true)]
        public AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition;

        [Inline(true)] [DrawNested] public DisarmSystemDefinition DisarmSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public GrabObjectActionInherentData SelectableGrabActionDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)] [DrawNested]
        public SelectableObjectSystemDefinition SelectableObjectSystemDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject parent)
        {
            return new TestAttractiveObject(InteractiveGameObjectFactory.Build(parent), this);
        }
    }
}