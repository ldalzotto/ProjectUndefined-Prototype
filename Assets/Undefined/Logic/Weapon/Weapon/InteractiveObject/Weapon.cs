using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace Weapon
{
    public class Weapon : CoreInteractiveObject
    {
        private FiringProjectileSystem FiringProjectileSystem;

        public Weapon(IInteractiveGameObject IInteractiveGameObject, WeaponDefinition WeaponDefinition)
        {
            this.FiringProjectileSystem = new FiringProjectileSystem(this, WeaponDefinition);
            this.BaseInit(IInteractiveGameObject, false);
        }

        public override void Init()
        {
            WeaponCreatedEvent.Get().OnWeaponCreated(this);
        }

        public void SpawnFiredProjectile(TransformStruct StartTransform)
        {
            this.FiringProjectileSystem.SpawnFiredProjectile(StartTransform);
        }
    }

    class FiringProjectileSystem
    {
        private WeaponRecoilTimeManager _weaponRecoilTimeManager = WeaponRecoilTimeManager.Get();
        private SpawnFiringProjectileEvent SpawnFiringProjectileEvent = SpawnFiringProjectileEvent.Get();

        private Weapon WeaponRef;
        private WeaponDefinition WeaponDefinition;

        public FiringProjectileSystem(Weapon WeaponRef, WeaponDefinition weaponDefinition)
        {
            this.WeaponRef = WeaponRef;
            WeaponDefinition = weaponDefinition;
        }

        public void SpawnFiredProjectile(TransformStruct StartTransform)
        {
            if (_weaponRecoilTimeManager.AuthorizeFiringAProjectile(this.WeaponRef))
            {
                var FiringProjectileInitializerPrefab = this.WeaponDefinition.FiringProjectileInitializerPrefab;
                var FiringProjectileInitializer = MonoBehaviour.Instantiate(FiringProjectileInitializerPrefab);
                FiringProjectileInitializer.Init();
                var FiredProjectile = FiringProjectileInitializer.GetCreatedFiredProjectile();
                var ProjectileSpawnLocalPosition = StartTransform.WorldPosition;
                var FiredProjectileTransform = FiredProjectile.InteractiveGameObject.GetTransform();
                // Eq (2)
                FiredProjectile.InteractiveGameObject.InteractiveGameObjectParent.transform.position = StartTransform.WorldPosition;
                FiredProjectile.InteractiveGameObject.InteractiveGameObjectParent.transform.eulerAngles = StartTransform.WorldRotationEuler;
                this.SpawnFiringProjectileEvent.OnFiringProjectileSpawned(this.WeaponRef, this.WeaponDefinition.RecoilTime);
            }
        }
    }
}