﻿using System;
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
        VisibilityProbe VisibilityProbe { get; }
        void GenerateVisibilityProbes(VisibilityProbeDefinition VisibilityProbeDefinition, BoxCollider BoxCollider);

        /// <summary>
        /// Hide all renderers only
        /// </summary>
        void Hide();

        /// <summary>
        /// Show all renderers only
        /// </summary>
        void Show();

        Bounds GetAverageModelWorldBounds();
        BoxCollider GetLogicColliderAsBox();
        TransformStruct GetTransform();
        Matrix4x4 GetLocalToWorld();
        Matrix4x4 GetWorldToLocal();
        BoxDefinition GetLogicColliderBoxDefinition();
        string GetAssociatedGameObjectName();


        void CreateAgent(AIAgentDefinition AIAgentDefinition);
        void CreateLogicCollider(InteractiveObjectBoxLogicColliderDefinitionStruct InteractiveObjectBoxLogicColliderDefinitionStruct, int layer = 0);
        void CreateLogicCollider(InteractiveObjectSphereLogicColliderDefinitionStruct InteractiveObjectSphereLogicColliderDefinitionStruct, int layer = 0);
    }
}