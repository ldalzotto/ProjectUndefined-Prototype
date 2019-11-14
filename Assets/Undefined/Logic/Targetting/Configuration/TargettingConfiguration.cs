using System;
using OdinSerializer;
using UnityEngine;

namespace Targetting
{
    [Serializable]
    public class TargettingConfiguration : SerializedScriptableObject
    {
        public GameObject TargetCursorPrefab;
        
        /// <summary>
        /// Once the <see cref="TargetCursorPrefab"/> is instanciated, it's position is offsetted from the Player forward position <see cref="TargetCursorSystem.OffsetTargetCursorPositionAtStart"/>.
        /// This value is the World length of this offset.
        /// </summary>
        public float TargetCursorInitialOffset;
    }
}