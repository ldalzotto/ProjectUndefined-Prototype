using System.Collections.Generic;
using CoreGame;
using GeometryIntersection;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObjects_Interfaces
{
    public interface IInteractiveGameObject
    {
        GameObject InteractiveGameObjectParent { get; }
        ExtendedBounds AverageModelLocalBounds { get; }
        Animator Animator { get; }
        List<Renderer> Renderers { get; }
        bool IsVisible();
        Collider LogicCollider { get; }
        Rigidbody PhysicsRigidbody { get; }
        Collider PhysicsCollider { get; }
        NavMeshAgent Agent { get; }

        Bounds GetAverageModelWorldBounds();
        BoxCollider GetLogicColliderAsBox();
        TransformStruct GetTransform();
        TransformStruct GetLogicColliderCenterTransform();
        Matrix4x4 GetLocalToWorld();
        BoxDefinition GetLogicColliderBoxDefinition();
        string GetAssociatedGameObjectName();

        void CreateAgent(AIAgentDefinition AIAgentDefinition);
        void CreateLogicCollider(InteractiveObjectBoxLogicColliderDefinition interactiveObjectLogicColliderDefinition, int layer = 0);
    }
}