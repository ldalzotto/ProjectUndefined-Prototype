using System;
using OdinSerializer;
using UnityEngine;

namespace VisualFeedback
{
    [Serializable]
    [CreateAssetMenu(fileName = "DottedLineStaticConfiguration", menuName = "Configuration/PuzzleGame/DottedLineConfiguration/DottedLineStaticConfiguration", order = 1)]
    public class DottedLineStaticConfiguration : SerializedScriptableObject
    {
        public Shader BaseDottedLineShader;
        public Mesh RangeDiamondMesh;
    }
}