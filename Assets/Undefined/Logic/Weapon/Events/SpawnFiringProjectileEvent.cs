using CoreGame;

namespace Weapon
{
    public class SpawnFiringProjectileEvent : GameSingleton<SpawnFiringProjectileEvent>
    {
        public void OnFiringProjectileSpawned(Weapon sourceWeapon, float recoilTime)
        {
            WeaponRecoilTimeManager.Get().OnFiredProjectileSpawned(sourceWeapon, recoilTime);
        }
    }
}