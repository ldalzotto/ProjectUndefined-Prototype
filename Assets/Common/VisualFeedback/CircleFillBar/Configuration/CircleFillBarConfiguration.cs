using OdinSerializer;
using UnityEngine;

namespace VisualFeedback
{
    public class CircleFillBarConfiguration : SerializedScriptableObject
    {
        [Header("Circle Progression Material")]
        public Material CircleProgressionMaterial;

        public Mesh ForwardQuadMesh;
    }
}