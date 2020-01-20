﻿using System.Collections.Generic;
using CoreGame;
using GeometryIntersection;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObjects
{
    public static class InteractiveGameObjectFactory
    {
        public static IInteractiveGameObject Build(GameObject InteractiveGameObjectParent)
        {
            return new InteractiveGameObject(InteractiveGameObjectParent);
        }
    }

    internal class InteractiveGameObject : IInteractiveGameObject
    {
        public InteractiveGameObject(GameObject InteractiveGameObjectParent)
        {
            var childRenderers = InteractiveGameObjectParent.GetComponentsInChildren<Renderer>();
            if (childRenderers != null) AverageModelLocalBounds = BoundsHelper.GetAverageRendererBoundsLocal(childRenderers, InteractiveGameObjectParent.transform.worldToLocalMatrix);

            Animator = InteractiveGameObjectParent.GetComponent<Animator>();
            if (Animator == null) Animator = InteractiveGameObjectParent.GetComponentInChildren<Animator>();

            this.InteractiveGameObjectParent = InteractiveGameObjectParent;
            Renderers = RendererRetrievableHelper.GetAllRederers(this.InteractiveGameObjectParent, false);

            Agent = InteractiveGameObjectParent.GetComponent<NavMeshAgent>();
            InitAgent();

            PhysicsRigidbody = this.InteractiveGameObjectParent.GetComponent<Rigidbody>();
            PhysicsCollider = this.InteractiveGameObjectParent.GetComponent<Collider>();
        }

        public GameObject InteractiveGameObjectParent { get; private set; }
        public void Hide()
        {
            if (this.Renderers != null)
            {
                for (var i = 0; i < this.Renderers.Count; i++)
                {
                    this.Renderers[i].enabled = false;
                }
            }
        }

        public void Show()
        {
            if (this.Renderers != null)
            {
                for (var i = 0; i < this.Renderers.Count; i++)
                {
                    this.Renderers[i].enabled = true;
                }
            }
        }

        public Bounds GetAverageModelWorldBounds()
        {
            return this.AverageModelLocalBounds.Bounds.Mul(this.InteractiveGameObjectParent.transform.localToWorldMatrix);
        }

        public BoxCollider GetLogicColliderAsBox()
        {
            return (BoxCollider) LogicCollider;
        }

        public string GetAssociatedGameObjectName()
        {
            return this.InteractiveGameObjectParent.name;
        }

        public void CreateAgent(AIAgentDefinition AIAgentDefinition)
        {
            if (Agent == null)
            {
                Agent = InteractiveGameObjectParent.AddComponent<NavMeshAgent>();
                Agent.areaMask = 1 << NavMesh.GetAreaFromName(NavMeshConstants.WALKABLE_LAYER);
                Agent.stoppingDistance = AIAgentDefinition.AgentStoppingDistance;
                Agent.radius = AIAgentDefinition.AgentRadius;
                Agent.height = AIAgentDefinition.AgentHeight;
                Agent.acceleration = 99999999f;
                Agent.angularSpeed = 99999999f;
            }

            InitAgent();
        }

        public void CreateLogicCollider(InteractiveObjectBoxLogicColliderDefinitionStruct InteractiveObjectBoxLogicColliderDefinitionStruct, int layer = 0)
        {
            if (InteractiveObjectBoxLogicColliderDefinitionStruct.Enabled)
            {
                var LogicColliderObject = new GameObject("LogicCollider");
                LogicColliderObject.layer = layer;
                LogicColliderObject.transform.parent = InteractiveGameObjectParent.transform;
                LogicColliderObject.transform.localPosition = Vector3.zero;
                LogicColliderObject.transform.localRotation = Quaternion.identity;
                LogicColliderObject.transform.localScale = Vector3.one;

                LogicCollider = LogicColliderObject.AddComponent<BoxCollider>();
                LogicCollider.isTrigger = true;
                ((BoxCollider) LogicCollider).center = InteractiveObjectBoxLogicColliderDefinitionStruct.LocalCenter;
                ((BoxCollider) LogicCollider).size = InteractiveObjectBoxLogicColliderDefinitionStruct.LocalSize;

                if (InteractiveObjectBoxLogicColliderDefinitionStruct.HasRigidBody)
                {
                    var rb = LogicColliderObject.AddComponent<Rigidbody>();
                    rb.interpolation = InteractiveObjectBoxLogicColliderDefinitionStruct.RigidbodyInterpolation;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }

        public TransformStruct GetTransform()
        {
            return new TransformStruct(InteractiveGameObjectParent.transform);
        }
        
        public Matrix4x4 GetLocalToWorld()
        {
            return InteractiveGameObjectParent.transform.localToWorldMatrix;
        }

        public Matrix4x4 GetWorldToLocal()
        {
            return InteractiveGameObjectParent.transform.worldToLocalMatrix;
        }

        public BoxDefinition GetLogicColliderBoxDefinition()
        {
            return new BoxDefinition(GetLogicColliderAsBox());
        }

        private void InitAgent()
        {
            if (Agent != null)
            {
                Agent.updatePosition = false;
                Agent.updateRotation = false;
            }
        }

        #region Properties

        public ExtendedBounds AverageModelLocalBounds { get; private set; }
        public Animator Animator { get; private set; }
        public List<Renderer> Renderers { get; private set; }

        public bool IsVisible()
        {
            var interactiveObjectRenderers = this.Renderers;
            for (var i = 0; i < interactiveObjectRenderers.Count; i++)
            {
                if (interactiveObjectRenderers[i].isVisible)
                {
                    return true;
                }
            }

            return false;
        }

        public Collider LogicCollider { get; private set; }

        public Rigidbody PhysicsRigidbody { get; private set; }
        public Collider PhysicsCollider { get; private set; }
        public NavMeshAgent Agent { get; private set; }

        #endregion
    }
}