using System;
using InteractiveObjectAction;
using InteractiveObjects;

namespace ProjectileDeflection
{
    [Serializable]
    public class ProjectileDeflectionTrackingInteractiveObjectActionInherentData : InteractiveObjectActionInherentData
    {
        /// <summary>
        /// This radius is the radius of the sphere (where center is the center of the logic collider) that is used
        /// to detect projectile objects to deflect
        /// </summary>
        [WireCircleWorldAttribute(B = 1f, UseTransform = true, RadiusFieldName = nameof(ProjectileDetectionRadius))]
        public float ProjectileDetectionRadius;
        
        public override string InteractiveObjectActionUniqueID
        {
            get { return ProjectileDeflectionTrackingInteractiveObjectAction.ProjectileDeflectionSystemUniqueID; }
        }

        public override AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput)
        {
            return new ProjectileDeflectionTrackingInteractiveObjectAction((ProjectileDeflectionSystemInput) interactiveObjectActionInput);
        }

        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            return new ProjectileDeflectionSystemInput(this, AssociatedInteractiveObject);
        }
    }
}