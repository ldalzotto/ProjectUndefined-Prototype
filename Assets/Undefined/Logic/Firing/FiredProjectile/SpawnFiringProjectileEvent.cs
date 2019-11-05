using CoreGame;

namespace Firing
{
    public class SpawnFiringProjectileEvent : GameSingleton<SpawnFiringProjectileEvent>
    {
        public void OnFiringProjectileSpawned(float recoilTime)
        {
            FiringRecoilTimeManager.Get().OnProjectileSpawned(recoilTime);
        }
    }
}