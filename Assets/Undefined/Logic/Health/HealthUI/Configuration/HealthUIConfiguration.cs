using System;
using OdinSerializer;
using UnityEngine;

namespace Health
{
    [Serializable]
    public class HealthUIConfiguration : SerializedScriptableObject
    {
        public GameObject HealthUIPrefab;
    }
}