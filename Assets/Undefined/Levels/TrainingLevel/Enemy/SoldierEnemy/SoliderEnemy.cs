using AIObjects;
using Damage;
using Health;
using InteractiveObject_Animation;
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
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystem;

        public SoliderEnemy(IInteractiveGameObject parent, SoliderEnemyDefinition SoliderEnemyDefinition)
        {
            parent.CreateLogicCollider(SoliderEnemyDefinition.InteractiveObjectBoxLogicColliderDefinition);
            parent.CreateAgent(SoliderEnemyDefinition.AIAgentDefinition);
            this.interactiveObjectTag = new InteractiveObjectTag() {IsTakingDamage = true};
            BaseInit(parent);
            this.HealthSystem = new HealthSystem(SoliderEnemyDefinition.HealthSystemDefinition, this.OnHealthBelowZero);
            this._stunningDamageDealerReceiverSystem = new StunningDamageDealerReceiverSystem(SoliderEnemyDefinition.stunningDamageDealerReceiverSystemDefinition, this.HealthSystem, this.OnStunningDamageDealingStarted, this.OnStunningDamageDealingEnded);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(this, new WeaponHandlingSystemInitializationData(this, SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponFirePointOriginLocal,
                SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, SoliderEnemyDefinition.AITransformMoveManagerComponentV3, this.OnAIDestinationReached,
                (unscaledSpeed) => this.BaseObjectAnimatorPlayableSystem.SetUnscaledObjectSpeed(unscaledSpeed));
            this.BaseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimatorPlayable, SoliderEnemyDefinition.LocomotionAnimation);
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
                this.AnimatorPlayable.Stop();
            }
        }

        private void OnStunningDamageDealingEnded()
        {
            foreach (var renderer in this.InteractiveGameObject.Renderers)
            {
                renderer.material.SetColor("_BaseColor", Color.white);
                this.AnimatorPlayable.Play();
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

        private void OnAIDestinationReached()
        {
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