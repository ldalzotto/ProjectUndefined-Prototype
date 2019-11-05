using Firing;
using InteractiveObjects;
using UnityEngine;

namespace Weapon
{
    public class WeaponDefinition : AbstractInteractiveObjectV2Definition
    {
        public float RecoilTime;
        public FiredProjectileInitializer FiringProjectileInitializerPrefab;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new Weapon(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}