using System;
using OdinSerializer;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    public class WeaponHandlingSystemDefinition : SerializedScriptableObject
    {
        public Vector3 WeaponFirePointOriginLocal;
        public WeaponDefinition WeaponDefinition;
    }
}