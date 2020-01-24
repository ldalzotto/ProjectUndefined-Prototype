using System;
using InteractiveObjects;
using OdinSerializer;
using UnityEngine;

namespace EnemySpawning
{
    [Serializable]
    public class EnemySpawningInteractiveObjectDefinition : AbstractInteractiveObjectV2Definition
    {
        public InteractiveObjectInitializer EnemyInteractiveObjectPrefab;
        public int MinEnemyAtSameTime;
        public int MaxEneemyAtSameTime;
        public float DeltaSecondsBetweenSpawnTry;
        
        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new EnemySpawningInteractiveObject(this);
        }
    }
}