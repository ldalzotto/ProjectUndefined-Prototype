using System;
using System.Collections;
using System.Collections.Generic;
using AIObjects;
using HealthGlobe;
using InteractiveObjectAction;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using ProjectileDeflection;
using SequencedAction;
using Tests;
using Tests.TestScenario;
using UnityEngine;
using UnityEngine.AI;
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
                    .Then(new HealthGlobeRecoveryScenarioAction(playerObject))
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

                    if (this._playerInteractiveObject is IEM_InteractiveObjectActionPlayerSystem_Retriever IEM_InteractiveObjectActionPlayerSystem_Retriever)
                    {
                        var projectileDeflectionTrackingInteractiveObjectAction = IEM_InteractiveObjectActionPlayerSystem_Retriever.InteractiveObjectActionPlayerSystem.GetPlayingPlayerActionReference(ProjectileDeflectionTrackingInteractiveObjectAction.ProjectileDeflectionSystemUniqueID)
                            as ProjectileDeflectionTrackingInteractiveObjectAction;
                        if (projectileDeflectionTrackingInteractiveObjectAction != null)
                        {
                            if (Vector3.Distance(this.SpawnedProjectile.InteractiveGameObject.GetTransform().WorldPosition, worldCenter) <= projectileDeflectionTrackingInteractiveObjectAction.GetProjectileDetectionRadius())
                            {
                                this.cor = Coroutiner.Instance.StartCoroutine(this.WaitFrame());
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator WaitFrame()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.Skill1DownHold = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.Skill1DownHold = false;
            this.ended = true;
        }
    }

    class HealthGlobeRecoveryScenarioAction : ASequencedAction
    {
        private PlayerInteractiveObject PlayerInteractiveObject;

        public HealthGlobeRecoveryScenarioAction(PlayerInteractiveObject playerInteractiveObject)
        {
            PlayerInteractiveObject = playerInteractiveObject;
        }

        public override void FirstExecutionAction()
        {
        }

        private bool isEnded;

        public override bool ComputeFinishedConditions()
        {
            return this.isEnded;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        private HealthGlobeInteractiveObject TrackedHealthGlobeInteractiveObject;

        public override void Tick(float d)
        {
            if (this.TrackedHealthGlobeInteractiveObject == null)
            {
                foreach (var interactiveObject in InteractiveObjectV2Manager.Get().InteractiveObjects)
                {
                    if (interactiveObject is HealthGlobeInteractiveObject HealthGlobeInteractiveObject)
                    {
                        if (!HealthGlobeInteractiveObject.IsMoving())
                        {
                            var destinationCalculationResult = this.PlayerInteractiveObject.SetDestination(new ForwardAgentMovementCalculationStrategy(new AIDestination(interactiveObject.InteractiveGameObject.GetTransform().WorldPosition, null)));

                            if (destinationCalculationResult == NavMeshPathStatus.PathComplete || destinationCalculationResult == NavMeshPathStatus.PathPartial)
                            {
                                this.TrackedHealthGlobeInteractiveObject = HealthGlobeInteractiveObject;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.TrackedHealthGlobeInteractiveObject.IsAskingToBeDestroyed)
                {
                    this.isEnded = true;
                }
            }
        }
    }
}