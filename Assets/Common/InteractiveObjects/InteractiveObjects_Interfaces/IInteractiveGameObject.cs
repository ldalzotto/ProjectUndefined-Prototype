using System.Collections.Generic;
using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObjects_Interfaces
{
    public interface IInteractiveGameObject
    {
        GameObject InteractiveGameObjectParent { get; }
        ExtendedBounds AverageModelBounds { get; }
        //TODO -> Animator should be deleted and replaced by PlayableAnimator
        Animator Animator { get; }
        List<Renderer> Renderers { get; }
        Collider LogicCollider { get; }
        Rigidbody PhysicsRigidbody { get; }
        Collider PhysicsCollider { get; }
        NavMeshAgent Agent { get; }

        BoxCollider GetLogicColliderAsBox();
        TransformStruct GetTransform();
        TransformStruct GetLogicColliderCenterTransform();
        Matrix4x4 GetLocalToWorld();
        BoxDefinition GetLogicColliderBoxDefinition();

        void CreateAgent(AIAgentDefinition AIAgentDefinition);
        void CreateLogicCollider(InteractiveObjectLogicColliderDefinition interactiveObjectLogicColliderDefinition);
    }
}