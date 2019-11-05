using UnityEngine;

namespace Firing
{
    class FiredProjectileMovementSystem
    {
        private FiredProjectileDefinition FiredProjectileDefinition;
        private FiredProjectile FiredProjectileRef;
        private Transform FiredProjectileTransform;

        private float CrossedDistance;

        public FiredProjectileMovementSystem(FiredProjectileDefinition firedProjectileDefinition, FiredProjectile FiredProjectileRef)
        {
            FiredProjectileDefinition = firedProjectileDefinition;
            this.FiredProjectileRef = FiredProjectileRef;
            this.FiredProjectileTransform = FiredProjectileRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
            this.CrossedDistance = 0f;
        }

        public void Tick(float d)
        {
            var oldPosition = this.FiredProjectileTransform.position;
            this.FiredProjectileTransform.position += this.FiredProjectileDefinition.Speed * this.FiredProjectileTransform.forward * d; // Eq FiredProjectile.html -> (1)
            this.CrossedDistance += Vector3.Distance(oldPosition, this.FiredProjectileTransform.position);
            if (this.CrossedDistance >= this.FiredProjectileDefinition.MaxDistance) // Eq FiredProjectile.html -> (2)
            {
                this.FiredProjectileRef.AskToDestroy();
            }
        }
    }
}