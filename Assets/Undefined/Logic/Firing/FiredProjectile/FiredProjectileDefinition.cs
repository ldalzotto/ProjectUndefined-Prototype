using Damage;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using UnityEngine;

namespace Firing
{
    [SceneHandleDraw]
    public class FiredProjectileDefinition : SerializedScriptableObject
    {
        public GameObject FiredProjectileModelPrefab;
        [DrawNested] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition;
        public float Speed;
        public float MaxDistance;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public DamageDealerSystemDefinition DamageDealerSystemDefinition;

        public FiredProjectile BuildFiredProjectile(CoreInteractiveObject WeaponHolder)
        {
            var FiredProjectileModel = Instantiate(this.FiredProjectileModelPrefab);
            return new FiredProjectile(InteractiveGameObjectFactory.Build(FiredProjectileModel), this, WeaponHolder);
        }
    }
}