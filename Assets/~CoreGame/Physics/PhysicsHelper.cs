using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class PhysicsHelper
    {
        public static bool PhysicsRayInContactWithColliders(Ray ray, Vector3 targetPoint, Collider[] colliders)
        {
            var raycastHits = Physics.RaycastAll(ray, Vector3.Distance(ray.origin, targetPoint));
            for (var i = 0; i < raycastHits.Length; i++)
            {
                foreach (var collider in colliders)
                {
                    if (raycastHits[i].collider.GetInstanceID() == collider.GetInstanceID())
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        public static bool RaycastToDownVertically(Collider collider, Rigidbody rigidbody, int layerMask,out RaycastHit hit)
        {
            var rayDistance = Mathf.Abs(rigidbody.transform.localPosition.y - collider.bounds.center.y) * 2;
            var startPosition = collider.bounds.center;

            Debug.DrawLine(startPosition, (startPosition + Vector3.down * rayDistance), Color.blue);
            var hitted = Physics.Raycast(startPosition, Vector3.down, out hit, rayDistance, layerMask);
            Debug.DrawLine(hit.point, hit.point + (hit.normal * 5), Color.magenta);
            return hitted;
        }
    }

}
