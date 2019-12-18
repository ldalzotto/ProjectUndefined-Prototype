using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
  public class SphereRangeObjectV2 : RangeObjectV2
    {
        private SphereRangeObjectInitialization SphereRangeObjectInitialization;

        public SphereRangeObjectV2(GameObject AssociatedGameObject, SphereRangeObjectInitialization SphereRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            this.RangeType = RangeType.SPHERE;
            this.SphereRangeObjectInitialization = SphereRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.SphereRangeObjectInitialization, this, AssociatedInteractiveObject, objectName);
            this.SphereBoundingCollider = (SphereCollider) RangeGameObjectV2.BoundingCollider;
            base.Init(RangeGameObjectV2, SphereRangeObjectInitialization);
        }

        public SphereCollider SphereBoundingCollider { get; private set; }
    }
}