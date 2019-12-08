using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace Weapon
{
    public class Weapon : CoreInteractiveObject
    {
        private FiringProjectileSystem FiringProjectileSystem;
        public CoreInteractiveObject WeaponHolder { get; private set; }

        public Weapon(IInteractiveGameObject IInteractiveGameObject, WeaponDefinition WeaponDefinition, CoreInteractiveObject weaponHolder)
        {
            this.WeaponHolder = weaponHolder;
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

        public override float GetFiredProjectileMaxRange()
        {
            return this.FiringProjectileSystem.GetProjectileMaxRange();
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
                var FiredProjectile = this.WeaponDefinition.FiredProjectileDefinition.BuildFiredProjectile(this.WeaponRef.WeaponHolder);
                var ProjectileSpawnLocalPosition = StartTransform.WorldPosition;
                var FiredProjectileTransform = FiredProjectile.InteractiveGameObject.GetTransform();
                // Eq (2)
                FiredProjectile.InteractiveGameObject.InteractiveGameObjectParent.transform.position = StartTransform.WorldPosition;
                FiredProjectile.InteractiveGameObject.InteractiveGameObjectParent.transform.eulerAngles = StartTransform.WorldRotationEuler;
                this.SpawnFiringProjectileEvent.OnFiringProjectileSpawned(FiredProjectile, this.WeaponRef, this.WeaponDefinition.RecoilTime);
            }
        }

        public float GetProjectileMaxRange()
        {
            return this.WeaponDefinition.FiredProjectileDefinition.MaxDistance;
        }
    }
}