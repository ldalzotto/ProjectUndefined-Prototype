using Damage;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace Firing
{
    [SceneHandleDraw]
    public class FiredProjectileDefinition : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition;
        public float Speed;
        public float MaxDistance;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public DamageDealerSystemDefinition DamageDealerSystemDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new FiredProjectile(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}