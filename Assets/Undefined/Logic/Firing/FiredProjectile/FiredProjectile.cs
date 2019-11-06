using System.Collections.Generic;
using Damage;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace Firing
{
    public class FiredProjectile : CoreInteractiveObject
    {
        private FiredProjectileMovementSystem FiredProjectileMovementSystem;
        private DamageDealerEmitterSystem _damageDealerEmitterSystem;
        private CoreInteractiveObject WeaponHolder;

        public FiredProjectile(IInteractiveGameObject parent, FiredProjectileDefinition FiredProjectileDefinition, CoreInteractiveObject weaponHolder)
        {
            this.WeaponHolder = weaponHolder;
            parent.CreateLogicCollider(FiredProjectileDefinition.InteractiveObjectBoxLogicColliderDefinition);
            this.BaseInit(parent, true);
            this.FiredProjectileMovementSystem = new FiredProjectileMovementSystem(FiredProjectileDefinition, this);
            this._damageDealerEmitterSystem = new DamageDealerEmitterSystem(this, FiredProjectileDefinition.damageDealerEmitterSystemDefinition, this.OnDamageDealtToOther, IgnoredInteractiveObjects: new List<CoreInteractiveObject>() {this.WeaponHolder});
            this.RegisterInteractiveObjectPhysicsEventListener(new InteractiveObjectPhysicsEventListenerDelegated((InteractiveObjectPhysicsTriggerInfo => InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsObstacle), this.OnObstacleTriggerEnter));
        }

        public override void Tick(float d)
        {
            this.FiredProjectileMovementSystem.Tick(d);
            base.Tick(d);
        }

        public override void Init()
        {
        }

        private void OnDamageDealtToOther(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AskToDestroy();
        }

        private void OnObstacleTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AskToDestroy();
        }

        public void AskToDestroy()
        {
            this.isAskingToBeDestroyed = true;
        }
    }
}