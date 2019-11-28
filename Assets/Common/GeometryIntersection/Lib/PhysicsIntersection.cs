using UnityEngine;

namespace GeometryIntersection
{
    public static class PhysicsIntersection
    {
        public static Collider[] BoxOverlapColliderWorld(BoxCollider BoxCollider)
        {
            Intersection.ExtractBoxColliderWorldPointsV2(new BoxDefinition(BoxCollider), out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
            var worldCenter = (C1 + C2 + C3 + C4 + C5 + C6 + C7 + C8) / 8f;
            var worldSize = new Vector3(Vector3.Distance(C1, C2), Vector3.Distance(C2, C3), Vector3.Distance(C1, C5));
            var worldCenterRotation = Quaternion.LookRotation((C5 - C1).normalized, (C2 - C3).normalized);

            return Physics.OverlapBox(worldCenter,
                worldSize * 0.5f,
                worldCenterRotation);
        }
    }
}