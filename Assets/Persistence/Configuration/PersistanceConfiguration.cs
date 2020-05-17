using System;
using OdinSerializer;
using UnityEngine;

namespace Persistence
{
    [Serializable]
    [CreateAssetMenu(fileName = "PersistanceConfiguration", menuName = "Configuration/CoreGame/GlobalLevelConfiguration/PersistanceConfiguration", order = 1)]
    public class PersistanceConfiguration : SerializedScriptableObject
    {
        public GameObject AutoSaveIconPrefab;
    }
}