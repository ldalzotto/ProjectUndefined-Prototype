using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace EnemySpawning
{
    public class EnemySpawningInteractiveObject : CoreInteractiveObject
    {
        private EnemySpawningInteractiveObjectDefinition EnemySpawningInteractiveObjectDefinition;

        public EnemySpawningInteractiveObject(EnemySpawningInteractiveObjectDefinition EnemySpawningInteractiveObjectDefinition)
        {
            this.EnemySpawningInteractiveObjectDefinition = EnemySpawningInteractiveObjectDefinition;
            base.BaseInit(null, true);
        }

        public override void Init()
        {
        }

        private int enemyCounter;
        private float currentTimer;

        public override void Tick(float d)
        {
            base.Tick(d);
            this.currentTimer += d;
            while (this.currentTimer >= this.EnemySpawningInteractiveObjectDefinition.DeltaSecondsBetweenSpawnTry)
            {
                this.currentTimer -= this.EnemySpawningInteractiveObjectDefinition.DeltaSecondsBetweenSpawnTry;
                if (this.enemyCounter <= this.EnemySpawningInteractiveObjectDefinition.MaxEneemyAtSameTime)
                {
                    var instanciatedEnemy = GameObject.Instantiate(this.EnemySpawningInteractiveObjectDefinition.EnemyInteractiveObjectPrefab).Init();
                    this.OnEnemyCreated(instanciatedEnemy);
                    instanciatedEnemy.RegisterInteractiveObjectDestroyedEventListener(this.OnEnemydestroyed);
                }
            }
        }

        private void OnEnemyCreated(CoreInteractiveObject EnemyCreated)
        {
            this.enemyCounter += 1;
        }

        private void OnEnemydestroyed(CoreInteractiveObject EnemyDestroyed)
        {
            this.enemyCounter -= 1;
        }
    }
}