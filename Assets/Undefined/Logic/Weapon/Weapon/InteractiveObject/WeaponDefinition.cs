using PlayerAim;
using InteractiveObjects;
using OdinSerializer;
using InteractiveObjectAction;
using UnityEngine;

namespace Weapon
{
    public class WeaponDefinition : SerializedScriptableObject
    {
        public GameObject WeaponModelPrefab;
        [Inline()] public FiredProjectileDefinition FiredProjectileDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public InteractiveObjectActionInherentData ProjectileFireActionDefinition;
        
        public Weapon BuildWeapon(CoreInteractiveObject WeaponHolder)
        {
            var weaponModelPrefab = Instantiate(this.WeaponModelPrefab);
            return new Weapon(InteractiveGameObjectFactory.Build(weaponModelPrefab), this, WeaponHolder);
        }

        public float GetFiredProjectileTravelSpeed()
        {
            return this.FiredProjectileDefinition.Speed;
        }
    }
}