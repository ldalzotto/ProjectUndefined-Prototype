using System.Collections.Generic;
using CoreGame;

namespace Weapon
{
    public class WeaponRecoilTimeManager : GameSingleton<WeaponRecoilTimeManager>
    {
        private Dictionary<Weapon, WeaponRecoilTimeSystem> WeaponRecoilTimeSystems = new Dictionary<Weapon, WeaponRecoilTimeSystem>();

        public void OnWeaponCreated(Weapon weapon)
        {
            var WeaponRecoilTimeSystem = new WeaponRecoilTimeSystem();
            this.WeaponRecoilTimeSystems[weapon] = WeaponRecoilTimeSystem;
        }

        public void OnFiredProjectileSpawned(Weapon sourceWeapon, float newRecoilTime)
        {
            this.WeaponRecoilTimeSystems[sourceWeapon].Reset(newRecoilTime);
        }

        public bool AuthorizeFiringAProjectile(Weapon weapon)
        {
            this.WeaponRecoilTimeSystems.TryGetValue(weapon, out WeaponRecoilTimeSystem WeaponRecoilTimeSystem);
            if (WeaponRecoilTimeSystem != null)
            {
                return WeaponRecoilTimeSystem.AuthorizeFiring();
            }

            return false;
        }

        public void Tick(float d)
        {
            foreach (var WeaponRecoilTimeSystem in WeaponRecoilTimeSystems.Values)
            {
                WeaponRecoilTimeSystem.Tick(d);
            }
        }
    }

    class WeaponRecoilTimeSystem
    {
        private float elapsedTime;
        private float weaponRecoilTime;

        public void Tick(float d)
        {
            this.elapsedTime += d;
        }

        public bool AuthorizeFiring()
        {
            return this.elapsedTime >= this.weaponRecoilTime;
        }

        public void Reset(float newRecoilTime)
        {
            this.elapsedTime = 0f;
            this.weaponRecoilTime = newRecoilTime;
        }
    }
}