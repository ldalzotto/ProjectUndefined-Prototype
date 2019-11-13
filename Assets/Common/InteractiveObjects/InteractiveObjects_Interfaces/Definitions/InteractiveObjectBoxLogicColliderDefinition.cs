using System;
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
    }
}