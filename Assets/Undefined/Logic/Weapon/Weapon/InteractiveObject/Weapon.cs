using Firing;
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
            this.FiringProjectileSystem = new FiringProjectileSystem(WeaponDefinition);
            this.BaseInit(IInteractiveGameObject, false);
        }

        public override void Init()
        {
        }

        public void SpawnFiredProjectile(TransformStruct StartTransform)
        {
            this.FiringProjectileSystem.SpawnFiredProjectile(StartTransform);
        }
    }

    class FiringProjectileSystem
    {
        private FiringRecoilTimeManager FiringRecoilTimeManager = FiringRecoilTimeManager.Get();
        private SpawnFiringProjectileEvent SpawnFiringProjectileEvent = SpawnFiringProjectileEvent.Get();

        private WeaponDefinition WeaponDefinition;

        public FiringProjectileSystem(WeaponDefinition weaponDefinition)
        {
            WeaponDefinition = weaponDefinition;
        }

        public void SpawnFiredProjectile(TransformStruct StartTransform)
        {
            if (FiringRecoilTimeManager.AuthorizeFiringAProjectile())
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
                this.SpawnFiringProjectileEvent.OnFiringProjectileSpawned(this.WeaponDefinition.RecoilTime);
            }
        }
    }
}