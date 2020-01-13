using System;
using System.Collections;
using System.Collections.Generic;
using InteractiveObjects;
using PlayerObject;
using SequencedAction;
using Tests;
using Tests.TestScenario;
using UnityEngine;
using Weapon;

namespace ProjectileDeflection_Test
{
    [Serializable]
    public class ProjectileDeflectionTestScenario : ATestScenarioDefinition
    {
        public override ASequencedAction[] BuildScenarioActions()
        {
            /// By default player has infinite health
            PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.DealDamage(-99999999, null);

            var playerObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
            var enemyObject = InteractiveObjectV2Manager.Get().InteractiveObjects.Find(o => o.InteractiveGameObject.GetAssociatedGameObjectName() == "GameObject");

            return new ASequencedAction[]
            {
                new ProjectileDeflectionTestScenarioAction(enemyObject, playerObject)
            };
        }
    }

    class ProjectileDeflectionTestScenarioAction : ASequencedAction
    {
        private CoreInteractiveObject EnemyObject;
        private PlayerInteractiveObject _playerInteractiveObject;
        private CoreInteractiveObject SpawnedProjectile;

        private Coroutine cor;
        private bool ended;

        public ProjectileDeflectionTestScenarioAction(CoreInteractiveObject EnemyObject, PlayerInteractiveObject PlayerInteractiveObject)
        {
            this.EnemyObject = EnemyObject;
            this._playerInteractiveObject = PlayerInteractiveObject;
            SpawnFiringProjectileEvent.Get().RegisterSpawnFiringProjectileEventListener(this.OnProjectileLaunched);
        }

        public override void FirstExecutionAction()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.ended;
        }

        public override void AfterFinishedEventProcessed()
        {
            SpawnFiringProjectileEvent.Get().UnRegisterSpawnFiringProjectileEventListener(this.OnProjectileLaunched);
            if (this.cor != null)
            {
                Coroutiner.Instance.StopCoroutine(this.cor);
                this.cor = null;
            }
        }

        public void OnProjectileLaunched(CoreInteractiveObject projectile, Weapon.Weapon sourceWeapon)
        {
            if (this.EnemyObject == sourceWeapon.WeaponHolder)
            {
                this.SpawnedProjectile = projectile;
                this.SpawnedProjectile.RegisterInteractiveObjectDestroyedEventListener((CoreInteractiveObject) => this.SpawnedProjectile = null);
                this.Tick(0f);
            }
        }

        public override void Tick(float d)
        {
            if (this.SpawnedProjectile != null)
            {
                if (_playerInteractiveObject.LowHealthPlayerSystem.IsHealthConsideredLow())
                {
                    var worldCenter = this._playerInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition().GetWorldCenter();

                    /*
                    if (Vector3.Distance(this.SpawnedProjectile.InteractiveGameObject.GetTransform().WorldPosition, worldCenter) <= this._playerInteractiveObject.ProjectileDeflectionSystem.GetProjectileDetectionRadius())
                    {
                        this.cor = Coroutiner.Instance.StartCoroutine(this.WaitFrame());
                    }
                    */
                }
            }
        }

        private IEnumerator WaitFrame()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.DeflectProjectileD = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.DeflectProjectileD = false;
            this.ended = true;
        }
    }
}