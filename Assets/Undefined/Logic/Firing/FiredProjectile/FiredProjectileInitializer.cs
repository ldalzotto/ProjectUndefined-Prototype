using InteractiveObjects;

namespace Firing
{
    public class FiredProjectileInitializer : InteractiveObjectInitializer
    {
        public FiredProjectile GetCreatedFiredProjectile()
        {
            return this.CreatedCoreInteractiveObject as FiredProjectile;
        }
    }
}