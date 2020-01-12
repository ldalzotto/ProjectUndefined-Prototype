using InteractiveObjects;

namespace PlayerAim
{
    /// <summary>
    /// Responsible of holding current reference of <see cref="WeaponHolder"/> and switching to another one.
    /// </summary>
    public class ProjectileWeaponHoldingSystem
    {
        /// <summary>
        /// The <see cref="WeaponHolder"/> is the interactive object that is considered to be the object that has
        /// fired this projectile.
        /// This value is used to disable friendly fire for example (<see cref="FiredProjectileHasTriggerEnter_DealsDamage_And_MustBeDestroyed"/>).
        /// </summary>
        private CoreInteractiveObject WeaponHolder;

        public ProjectileWeaponHoldingSystem(CoreInteractiveObject weaponHolder)
        {
            this.SwitchWeaponHolder(weaponHolder);
        }

        public void SwitchWeaponHolder(CoreInteractiveObject NewWeaponHolder)
        {
            this.WeaponHolder = NewWeaponHolder;
        }
        
        #region Data Retrieval

        /// <summary>
        /// /!\ It is not advised to store this value as a reference because <see cref="WeaponHolder"/> can be switched to another at runtime.
        /// <see cref="SwitchWeaponHolder"/>
        /// </summary>
        public CoreInteractiveObject GetWeaponHolder()
        {
            return this.WeaponHolder;
        }

        #endregion
    }
}