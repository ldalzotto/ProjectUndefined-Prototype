using OdinSerializer;
using UnityEngine;

namespace RangeObjects
{
    [CreateAssetMenu(fileName = "RangeRenderingConfiguration", menuName = "Configuration/PuzzleGame/RangeTypeConfiguration/RangeRenderingConfiguration", order = 0)]
    public class RangeRenderingConfiguration : SerializedScriptableObject
    {
        [SerializeField] private bool isRangeRenderingEnabled;

        [SerializeField] private Shader masterRangeShader;
        [SerializeField] private RangeTypeConfiguration rangeTypeConfiguration;
        public RangeTypeConfiguration RangeTypeConfiguration => rangeTypeConfiguration;
        public Shader MasterRangeShader => masterRangeShader;
        public bool IsRangeRenderingEnabled => isRangeRenderingEnabled;
    }
}