using CoreGame;

namespace Firing
{
    public class FiringRecoilTimeManager : GameSingleton<FiringRecoilTimeManager>
    {
        private float elapsedTime;
        private float projectileRecoilTime;

        public void OnProjectileSpawned(float recoilTime)
        {
            this.elapsedTime = 0f;
            this.projectileRecoilTime = recoilTime;
        }

        public bool AuthorizeFiringAProjectile()
        {
            return this.elapsedTime >= this.projectileRecoilTime; // Eq (1)
        }

        public void Tick(float d)
        {
            this.elapsedTime += d;
        }
    }
}