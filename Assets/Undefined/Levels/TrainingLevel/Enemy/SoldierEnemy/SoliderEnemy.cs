using Damage;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using Weapon;

namespace TrainingLevel
{
    public class SoliderEnemy : CoreInteractiveObject
    {
        [VE_Nested] private HealthSystem HealthSystem;
        [VE_Nested] private StunningDamageDealerReceiverSystem _stunningDamageDealerReceiverSystem;
        private WeaponHandlingSystem WeaponHandlingSystem;

        public SoliderEnemy(IInteractiveGameObject parent, SoliderEnemyDefinition SoliderEnemyDefinition)
        {
            parent.CreateLogicCollider(SoliderEnemyDefinition.InteractiveObjectBoxLogicColliderDefinition);
            this.interactiveObjectTag = new InteractiveObjectTag() {IsTakingDamage = true};
            BaseInit(parent);
            this.HealthSystem = new HealthSystem(SoliderEnemyDefinition.HealthSystemDefinition, this.OnHealthBelowZero);
            this._stunningDamageDealerReceiverSystem = new StunningDamageDealerReceiverSystem(SoliderEnemyDefinition.stunningDamageDealerReceiverSystemDefinition, this.HealthSystem, this.OnStunningDamageDealingStarted, this.OnStunningDamageDealingEnded);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(this, new WeaponHandlingSystemInitializationData(this, SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponFirePointOriginLocal,
                SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
        }

        public override void Tick(float d)
        {
            this._stunningDamageDealerReceiverSystem.Tick(d);
            if (!this._stunningDamageDealerReceiverSystem.IsStunned.GetValue())
            {
                //PUT all other logic
            }

            base.Tick(d);
        }

        private void OnStunningDamageDealingStarted()
        {
            foreach (var renderer in this.InteractiveGameObject.Renderers)
            {
                renderer.material.SetColor("_BaseColor", Color.red);
            }
        }

        private void OnStunningDamageDealingEnded()
        {
            foreach (var renderer in this.InteractiveGameObject.Renderers)
            {
                renderer.material.SetColor("_BaseColor", Color.white);
            }
        }

        public override void OnHealthBelowZero()
        {
            this.isAskingToBeDestroyed = true;
        }

        public override void DealDamage(float Damage)
        {
            this._stunningDamageDealerReceiverSystem.DealDamage(Damage);
        }

        #region Projectile Events

        public override void AskToFireAFiredProjectile()
        {
            this.WeaponHandlingSystem.AskToFireAFiredProjectile();
        }

        #endregion

        public override void Init()
        {
        }
    }
}