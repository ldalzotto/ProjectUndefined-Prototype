using System;
using AnimatorPlayable;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace AIObjects
{
    [Serializable]
    [SceneHandleDraw]
    public abstract class AbstractAIInteractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public AIAgentDefinition AIAgentDefinition;
        [DrawNested]
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectLogicCollider;
        public TransformMoveManagerComponentV3 TransformMoveManagerComponentV3;
        public A_AnimationPlayableDefinition LocomotionAnimation;
    }
}