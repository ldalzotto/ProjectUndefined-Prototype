using UnityEngine;
using System.Collections;
using AIObjects;
using InteractiveObjects;

namespace InteractiveObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "AggressiveObjectInitializerData", menuName = "Test/AggressiveObjectInitializerData", order = 1)]
    public class AggressiveObjectInitializerData : AbstractAIInteractiveObjectInitializerData
    {
        public AIPatrolSystemDefinition AIPatrolSystemDefinition;

        [Inline(createAtSameLevelIfAbsent: true)]
        public SightObjectSystemDefinition SightObjectSystemDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new AggressiveAIObject(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}