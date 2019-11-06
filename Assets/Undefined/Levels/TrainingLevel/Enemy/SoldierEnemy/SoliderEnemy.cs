using Damage;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using Weapon;

namespace TrainingLevel
{
    public class SoliderEnemy : CoreInteractiveObject
    {
        [VE_Nested] private HealthSystem HealthSystem;
        [VE_Nested] private StunningDamageDealingSystem StunningDamageDealingSystem;
        private WeaponHandlingSystem WeaponHandlingSystem;

        public SoliderEnemy(IInteractiveGameObject parent, SoliderEnemyDefinition SoliderEnemyDefinition)
        {
            parent.CreateLogicCollider(SoliderEnemyDefinition.InteractiveObjectBoxLogicColliderDefinition);
            this.interactiveObjectTag = new InteractiveObjectTag() {IsTakingDamage = true};
            BaseInit(parent);
            this.HealthSystem = new HealthSystem(SoliderEnemyDefinition.HealthSystemDefinition, this.OnHealthBelowZero);
            this.StunningDamageDealingSystem = new StunningDamageDealingSystem(SoliderEnemyDefinition.StunningDamageDealingSystemDefinition, this.HealthSystem, this.OnStunningDamageDealingStarted, this.OnStunningDamageDealingEnded);
            this.WeaponHandlingSystem = new WeaponHandlingSystem(this, new WeaponHandlingSystemInitializationData(this, SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponFirePointOriginLocal,
                SoliderEnemyDefinition.WeaponHandlingSystemDefinition.WeaponDefinition));
        }

        public override void Tick(float d)
        {
            this.StunningDamageDealingSystem.Tick(d);
            if (!this.StunningDamageDealingSystem.IsStunned.GetValue())
            {
                //PUT all other logic
            }

            base.Tick(d);
        }

        private void OnStunningDamageDealingStarted()
        {
        }

        private void OnStunningDamageDealingEnded()
        {
        }

        public override void OnHealthBelowZero()
        {
            this.isAskingToBeDestroyed = true;
        }

        public override void DealDamage(float Damage)
        {
            this.StunningDamageDealingSystem.DealDamage(Damage);
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