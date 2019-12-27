using Firing;
using InteractiveObjects;
using OdinSerializer;
using UnityEngine;

namespace Weapon
{
    public class WeaponDefinition : SerializedScriptableObject
    {
        public GameObject WeaponModelPrefab;
        public float RecoilTime;
        [Inline()]
        public FiredProjectileDefinition FiredProjectileDefinition;

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