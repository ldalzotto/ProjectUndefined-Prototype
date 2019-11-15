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

        public void FixedTick(float d)
        {
            var oldPosition = this.FiredProjectileTransform.position;
            /// /!\ Position displacement needs to be in FixedTick.
            this.FiredProjectileTransform.position += this.FiredProjectileDefinition.Speed * this.FiredProjectileTransform.forward * d;
            this.CrossedDistance += Vector3.Distance(oldPosition, this.FiredProjectileTransform.position);
            if (this.CrossedDistance >= this.FiredProjectileDefinition.MaxDistance) // Eq FiredProjectile.html -> (2)
            {
                this.FiredProjectileRef.AskToDestroy();
            }
        }
    }
}