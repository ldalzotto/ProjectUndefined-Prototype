using System;
using System.Runtime.CompilerServices;
using OdinSerializer;
using UnityEngine;

namespace InteractiveObjects_Interfaces
{
    [Serializable]
    [SceneHandleDraw]
    public class InteractiveObjectBoxLogicColliderDefinition : SerializedScriptableObject
    {
        public bool Enabled = true;
        public bool HasRigidBody = true;
        public RigidbodyInterpolation RigidbodyInterpolation;
        [WireBox(R = 1, G = 1, B = 0, CenterFieldName = nameof(InteractiveObjectBoxLogicColliderDefinition.LocalCenter),
            SizeFieldName = nameof(InteractiveObjectBoxLogicColliderDefinition.LocalSize))]
        public Vector3 LocalCenter;

        public Vector3 LocalSize;
        
        public static implicit operator InteractiveObjectBoxLogicColliderDefinitionStruct(InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition)
        { 
            return new InteractiveObjectBoxLogicColliderDefinitionStruct(InteractiveObjectBoxLogicColliderDefinition);
        }
    }

    /// <summary>
    /// This struct is used for manipulating <see cref="InteractiveObjectBoxLogicColliderDefinition"/> data without changing the original <see cref="ScriptableObject"/>.
    /// For example, we may want to force overriding a value via script.
    /// </summary>
    public struct InteractiveObjectBoxLogicColliderDefinitionStruct
    {
        public bool Enabled;
        public bool HasRigidBody;
        public RigidbodyInterpolation RigidbodyInterpolation;
        public Vector3 LocalCenter;
        public Vector3 LocalSize;

        public InteractiveObjectBoxLogicColliderDefinitionStruct(InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition)
        {
            Enabled = InteractiveObjectBoxLogicColliderDefinition.Enabled;
            HasRigidBody = InteractiveObjectBoxLogicColliderDefinition.HasRigidBody;
            RigidbodyInterpolation = InteractiveObjectBoxLogicColliderDefinition.RigidbodyInterpolation;
            LocalCenter = InteractiveObjectBoxLogicColliderDefinition.LocalCenter;
            LocalSize = InteractiveObjectBoxLogicColliderDefinition.LocalSize;
        }
    }
    
   
}