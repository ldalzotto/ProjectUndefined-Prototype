using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace Weapon
{
    public class Weapon : CoreInteractiveObject
    {
        private FiringProjectileSystem FiringProjectileSystem;
        private WeaponPositioningSystem WeaponPositioningSystem;
        public CoreInteractiveObject WeaponHolder { get; private set; }

        public Weapon(IInteractiveGameObject IInteractiveGameObject, WeaponDefinition WeaponDefinition, CoreInteractiveObject weaponHolder)
        {
            this.WeaponHolder = weaponHolder;
            this.BaseInit(IInteractiveGameObject, true);

            this.FiringProjectileSystem = new FiringProjectileSystem(this, WeaponDefinition);
            this.WeaponPositioningSystem = new WeaponPositioningSystem(this, this.WeaponHolder);
        }

        public override void Init()
        {
            WeaponCreatedEvent.Get().OnWeaponCreated(this);
        }

        public override void Tick(float d)
        {
            this.WeaponPositioningSystem.Tick(d);
        }

        public void SpawnFiredProjectile(TransformStruct StartTransform)
        {
            this.FiringProjectileSystem.SpawnFiredProjectile(StartTransform);
        }

        public override float GetFiredProjectileMaxRange()
        {
            return this.FiringProjectileSystem.GetProjectileMaxRange();
        }

        public float GetFiredProjectileTravelSpeed()
        {
            return this.FiringProjectileSystem.GetProjectileTravelSpeed();
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

        public float GetProjectileTravelSpeed()
        {
            return this.WeaponDefinition.GetFiredProjectileTravelSpeed();
        }
    }

    /// <summary>
    /// /!\ This system supposes that the WeaponHolder has an animator with the <see cref="BipedArmatureName.ItemHold_L"/> bone.
    /// Thus must be changed when we want to attach weapon to anything than Biped.
    /// </summary>
    struct WeaponPositioningSystem
    {
        private bool isEnabled;
        private CopyInteractiveObjectTransformConstraint CopyConstraint;

        public WeaponPositioningSystem(Weapon AssociatedWeapon, CoreInteractiveObject WeaponHolder)
        {
            this.isEnabled = default(bool);
            this.CopyConstraint = default(CopyInteractiveObjectTransformConstraint);
            if (WeaponHolder.InteractiveGameObject.Animator != null)
            {
                var itemholdLeftBone = WeaponHolder.InteractiveGameObject.Animator.gameObject.FindChildObjectRecursively(BipedArmatureConstants.GetBipedBoneName(BipedArmatureName.ItemHold_L));
                if (itemholdLeftBone != null)
                {
                    this.isEnabled = true;
                    this.CopyConstraint = new CopyInteractiveObjectTransformConstraint(AssociatedWeapon.InteractiveGameObject,
                        itemholdLeftBone.transform);
                }
            }
        }

        public void Tick(float d)
        {
            if (this.isEnabled)
            {
                this.CopyConstraint.ApplyConstraint();
            }
        }
    }
}