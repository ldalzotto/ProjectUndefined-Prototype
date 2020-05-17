using System;
using UnityEngine;

namespace LevelManagement_Interfaces
{
    [Serializable]
    public class LevelRangeEffectInherentData
    {
        public float DeltaIntensity = 0;
        [Range(-0.5f, 0.5f)] public float DeltaMixFactor = 0;
    }
}