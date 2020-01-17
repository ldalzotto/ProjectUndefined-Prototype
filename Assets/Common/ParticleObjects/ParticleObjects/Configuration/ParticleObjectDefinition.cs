using System;
using OdinSerializer;
using UnityEngine;

namespace ParticleObjects
{
    [Serializable]
    public class ParticleObjectDefinition : SerializedScriptableObject
    {
        public GameObject Prefab;
        
        public ParticleObject BuildParticleObject(string GameObjectName, Transform Parent, Nullable<Vector3> InitialLocalPosition = null, Nullable<Quaternion> InitialLocalQuaternion = null)
        {
            GameObject instanciatedObject = null;
            if (Prefab != null)
            {
                instanciatedObject = Instantiate(this.Prefab);
            }
            else
            {
                instanciatedObject = new GameObject(GameObjectName);
            }

            if (Parent != null)
            {
                instanciatedObject.transform.parent = Parent;
            }

            if (InitialLocalPosition.HasValue)
            {
                instanciatedObject.transform.localPosition = InitialLocalPosition.Value;
            }

            if (InitialLocalQuaternion.HasValue)
            {
                instanciatedObject.transform.localRotation = InitialLocalQuaternion.Value;
            }

            return new ParticleObject(this, ParticleGameObject.BuildFromGameObject(instanciatedObject));
        }
    }
}