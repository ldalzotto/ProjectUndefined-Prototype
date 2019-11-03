using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace Firing
{
    public class FiredProjectileDefinition : AbstractInteractiveObjectV2Definition
    {
        public float Speed;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new FiredProjectile(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }

    public class FiredProjectile : CoreInteractiveObject
    {
        private FiredProjectileMovementSystem FiredProjectileMovementSystem;

        public FiredProjectile(IInteractiveGameObject parent, FiredProjectileDefinition FiredProjectileDefinition)
        {
            this.BaseInit(parent, true);
            this.FiredProjectileMovementSystem = new FiredProjectileMovementSystem(FiredProjectileDefinition, this);
        }

        public override void Tick(float d)
        {
            this.FiredProjectileMovementSystem.Tick(d);    
            base.Tick(d);
        }

        public override void Init()
        {
        }
    }

    class FiredProjectileMovementSystem
    {
        private FiredProjectileDefinition FiredProjectileDefinition;
        private Transform FiredProjectileTransform;

        public FiredProjectileMovementSystem(FiredProjectileDefinition firedProjectileDefinition, FiredProjectile FiredProjectileRef)
        {
            FiredProjectileDefinition = firedProjectileDefinition;
            this.FiredProjectileTransform = FiredProjectileRef.InteractiveGameObject.InteractiveGameObjectParent.transform;
        }

        public void Tick(float d)
        {
            this.FiredProjectileTransform.position += this.FiredProjectileDefinition.Speed * this.FiredProjectileTransform.forward * d; // Eq (1)
        }
    }
}