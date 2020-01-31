using System;
using OdinSerializer;
using UnityEngine;

namespace InteractiveObjects_Interfaces
{
    [Serializable]
    [SceneHandleDraw]
    public class InteractiveObjectSphereLogicColliderDefinition : SerializedScriptableObject
    {
        public bool Enabled = true;
        public bool HasRigidBody = true;
        public RigidbodyInterpolation RigidbodyInterpolation;
        public Vector3 LocalCenter;
        public float Radius;

        public static implicit operator InteractiveObjectSphereLogicColliderDefinitionStruct(InteractiveObjectSphereLogicColliderDefinition InteractiveObjectSphereLogicColliderDefinition)
        {
            return new InteractiveObjectSphereLogicColliderDefinitionStruct(InteractiveObjectSphereLogicColliderDefinition);
        }
    }

    public struct InteractiveObjectSphereLogicColliderDefinitionStruct
    {
        public bool Enabled;
        public bool HasRigidBody;
        public RigidbodyInterpolation RigidbodyInterpolation;
        public Vector3 LocalCenter;
        public float Radius;

        public InteractiveObjectSphereLogicColliderDefinitionStruct(InteractiveObjectSphereLogicColliderDefinition InteractiveObjectSphereLogicColliderDefinition)
        {
            Enabled = InteractiveObjectSphereLogicColliderDefinition.Enabled;
            HasRigidBody = InteractiveObjectSphereLogicColliderDefinition.HasRigidBody;
            RigidbodyInterpolation = InteractiveObjectSphereLogicColliderDefinition.RigidbodyInterpolation;
            LocalCenter = InteractiveObjectSphereLogicColliderDefinition.LocalCenter;
            Radius = InteractiveObjectSphereLogicColliderDefinition.Radius;
        }
    }
}