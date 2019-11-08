﻿using Damage;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing
{
    [SceneHandleDraw]
    public class FiredProjectileDefinition : SerializedScriptableObject
    {
        public GameObject FiredProjectileModelPrefab;
        [DrawNested] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition;
        public float Speed;
        public float MaxDistance;

        [FormerlySerializedAs("DamageDealerSystemDefinition")] [Inline(CreateAtSameLevelIfAbsent = true)]
        public DamageDealerEmitterSystemDefinition damageDealerEmitterSystemDefinition;

        public FiredProjectile BuildFiredProjectile(CoreInteractiveObject WeaponHolder)
        {
            var FiredProjectileModel = Instantiate(this.FiredProjectileModelPrefab);
            return new FiredProjectile(InteractiveGameObjectFactory.Build(FiredProjectileModel), this, WeaponHolder);
        }
    }
}