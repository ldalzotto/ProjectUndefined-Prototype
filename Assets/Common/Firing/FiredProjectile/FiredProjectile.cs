﻿using Damage;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace Firing
{
    public class FiredProjectile : CoreInteractiveObject
    {
        private FiredProjectileMovementSystem FiredProjectileMovementSystem;
        private DamageDealerSystem DamageDealerSystem;

        public FiredProjectile(IInteractiveGameObject parent, FiredProjectileDefinition FiredProjectileDefinition)
        {
            parent.CreateLogicCollider(FiredProjectileDefinition.InteractiveObjectBoxLogicColliderDefinition);
            this.BaseInit(parent, true);
            this.FiredProjectileMovementSystem = new FiredProjectileMovementSystem(FiredProjectileDefinition, this);
            this.DamageDealerSystem = new DamageDealerSystem(this, FiredProjectileDefinition.DamageDealerSystemDefinition);
        }

        public override void Tick(float d)
        {
            this.FiredProjectileMovementSystem.Tick(d);
            base.Tick(d);
        }

        public override void Init()
        {
        }

        public void AskToDestroy()
        {
            this.isAskingToBeDestroyed = true;
        }
    }
}