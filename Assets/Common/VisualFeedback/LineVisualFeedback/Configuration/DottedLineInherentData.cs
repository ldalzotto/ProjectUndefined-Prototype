using System;
using UnityEngine;

namespace VisualFeedback
{
    [Serializable]
    [CreateAssetMenu(fileName = "DottedLineInherentData", menuName = "Configuration/PuzzleGame/DottedLineConfiguration/DottedLineInherentData", order = 1)]
    public class DottedLineInherentData : ScriptableObject
    {
        public DottedLineType DottedLineType;
        public float ModelScale;
        public Color BaseColor;
        public Color MovingColor;
        public float MovingWidth;
        public float DotPerUnitDistance;
    }

    public enum DottedLineType
    {
        CURVED,
        STRAIGHT
    }
}