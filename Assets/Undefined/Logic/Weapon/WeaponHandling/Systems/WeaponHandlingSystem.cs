using InteractiveObjects;
using UnityEngine;

namespace Weapon
{
    public class WeaponHandlingSystem
    {
        public WeaponHandlingSystemInitializationData WeaponHandlingSystemInitializationData;
        private Weapon WeaponReference;

        public WeaponHandlingSystem(WeaponHandlingSystemInitializationData weaponHandlingSystemInitializationData)
        {
            WeaponHandlingSystemInitializationData = weaponHandlingSystemInitializationData;
            var WeaponInitializer = MonoBehaviour.Instantiate(this.WeaponHandlingSystemInitializationData.WeaponInitializerPrefab);
            WeaponInitializer.Init();
            this.WeaponReference = WeaponInitializer.GetInitializedWeapon();
        }

        public void AskToFireAFiredProjectile()
        {
            var parent = WeaponHandlingSystemInitializationData.Parent;
            //Eq (1)
            var firedProjectilePosition = parent.InteractiveGameObject.GetTransform().WorldPosition + WeaponHandlingSystemInitializationData.WeaponFirePointOriginLocal;
            var firedProjectileRotation = parent.InteractiveGameObject.GetTransform().WorldRotationEuler;
            this.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation});
        }
    }

    public class WeaponHandlingSystemInitializationData
    {
        public CoreInteractiveObject Parent;
        public Vector3 WeaponFirePointOriginLocal;
        public WeaponInitializer WeaponInitializerPrefab;

        public WeaponHandlingSystemInitializationData(CoreInteractiveObject parent, Vector3 weaponFirePointOriginLocal, WeaponInitializer weaponInitializerPrefab)
        {
            Parent = parent;
            WeaponFirePointOriginLocal = weaponFirePointOriginLocal;
            WeaponInitializerPrefab = weaponInitializerPrefab;
        }
    }
}