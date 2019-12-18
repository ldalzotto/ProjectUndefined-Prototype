using System;
using OdinSerializer;
using UnityEngine;

namespace ProjectileDeflection
{
    [Serializable]
    public class ProjectileDeflectionGlobalConfiguration : SerializedScriptableObject
    {
        public GameObject DeflectionIconPrefab;
    }
}