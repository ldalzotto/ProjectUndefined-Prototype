using System;
using ConfigurationEditor;
using UnityEngine;

namespace VisualFeedback
{
    [Serializable]
    [CreateAssetMenu(fileName = "DottedLineConfiguration", menuName = "Configuration/PuzzleGame/DottedLineConfiguration/DottedLineConfiguration", order = 1)]
    public class DottedLineConfiguration : ConfigurationSerialization<DottedLineID, DottedLineInherentData>
    {
    }
}