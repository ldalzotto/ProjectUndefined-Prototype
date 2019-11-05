using Damage;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace TrainingLevel
{
    public class SoliderEnemy : CoreInteractiveObject
    {
        [VE_Nested] private HealthSystem HealthSystem;
        [VE_Nested] private StunningDamageDealingSystem StunningDamageDealingSystem;

        public SoliderEnemy(IInteractiveGameObject parent, SoliderEnemyDefinition SoliderEnemyDefinition)
        {
            parent.CreateLogicCollider(SoliderEnemyDefinition.InteractiveObjectBoxLogicColliderDefinition);
            this.interactiveObjectTag = new InteractiveObjectTag() {IsTakingDamage = true};
            BaseInit(parent);
            this.HealthSystem = new HealthSystem(SoliderEnemyDefinition.HealthSystemDefinition, this.OnHealthBelowZero);
            this.StunningDamageDealingSystem = new StunningDamageDealingSystem(SoliderEnemyDefinition.StunningDamageDealingSystemDefinition, this.HealthSystem, this.OnStunningDamageDealingStarted, this.OnStunningDamageDealingEnded);
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

        public override void Init()
        {
        }
    }
}