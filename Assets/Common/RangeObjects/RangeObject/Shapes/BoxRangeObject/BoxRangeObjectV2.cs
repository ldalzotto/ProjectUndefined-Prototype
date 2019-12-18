using GeometryIntersection;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    public class BoxRangeObjectV2 : RangeObjectV2
    {
        private BoxRangeObjectInitialization BoxRangeObjectInitialization;

        public BoxRangeObjectV2(GameObject AssociatedGameObject, BoxRangeObjectInitialization BoxRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            this.RangeType = RangeType.BOX;
            this.BoxRangeObjectInitialization = BoxRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.BoxRangeObjectInitialization, this, AssociatedInteractiveObject, objectName);
            base.Init(RangeGameObjectV2, BoxRangeObjectInitialization);
        }

        public BoxDefinition GetBoxBoundingColliderDefinition()
        {
            var BoxCollider = (BoxCollider) RangeGameObjectV2.BoundingCollider;
            return new BoxDefinition(BoxCollider);
        }

        public void SetLocalCenter(Vector3 localCenter)
        {
            var BoxCollider = (BoxCollider) RangeGameObjectV2.BoundingCollider;
            BoxCollider.center = localCenter;
        }

        public void SetLocalSize(Vector3 localSize)
        {
            var BoxCollider = (BoxCollider) RangeGameObjectV2.BoundingCollider;
            BoxCollider.size = localSize;
        }
    }
}