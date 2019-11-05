using InteractiveObjects;

namespace Weapon
{
    public class WeaponInitializer : InteractiveObjectInitializer
    {
        public Weapon GetInitializedWeapon()
        {
            return this.CreatedCoreInteractiveObject as Weapon;
        }
    }
}