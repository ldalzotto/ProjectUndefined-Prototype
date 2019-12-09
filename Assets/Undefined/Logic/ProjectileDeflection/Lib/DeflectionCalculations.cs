using InteractiveObjects;
using ProjectileDeflection_Interface;

namespace ProjectileDeflection
{
    public static class DeflectionCalculations
    {
        public static void ForwardDeflection(CoreInteractiveObject DelfectionActorObject, CoreInteractiveObject DeflectedInteractiveObject)
        {
            DeflectedInteractiveObject.SwitchWeaponHolder(DelfectionActorObject);
            DeflectedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward = -DeflectedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward;
        }
    }
}