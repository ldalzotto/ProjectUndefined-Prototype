using Unity.Mathematics;
using UnityEngine;

namespace CoreGame
{
    public struct BeziersControlPoints
    {
        private Vector3 p0;
        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;

        public BeziersControlPoints(BeziersControlPointsBuildInput BeziersControlPointsBuildInput)
        {
            p0 = BeziersControlPointsBuildInput.initialPosition;
            p3 = BeziersControlPointsBuildInput.targetPosition;

            if (BeziersControlPointsBuildInput.BeziersControlPointsShape == BeziersControlPointsShape.CURVED)
            {
                var upProjectedPath = Vector3.ProjectOnPlane(BeziersControlPointsBuildInput.targetPosition - p0, BeziersControlPointsBuildInput.normal);
                var upProjectedPathLength = upProjectedPath.magnitude;

                p1 = p0 + (upProjectedPath.normalized + BeziersControlPointsBuildInput.normal) * upProjectedPathLength / 3;
                p2 = p1 + (upProjectedPath.normalized) * upProjectedPathLength / 3;
            }
            else
            {
                Vector3 direction = (BeziersControlPointsBuildInput.targetPosition - BeziersControlPointsBuildInput.initialPosition).normalized;
                float distance = Vector3.Distance(BeziersControlPointsBuildInput.targetPosition, BeziersControlPointsBuildInput.initialPosition);

                p1 = p0 + (direction * distance / 3f);
                p2 = p1 + (direction * distance / 3f);
            }
        }

        public Vector3 ResolvePoint(float t)
        {
            return p0 * Mathf.Pow((1 - t), 3) + (3 * p1 * t * Mathf.Pow(1 - t, 2)) + (3 * p2 * Mathf.Pow(t, 2) * (1 - t)) + (p3 * Mathf.Pow(t, 3));
        }
    }

    public enum BeziersControlPointsShape
    {
        CURVED,
        STRAIGHT
    }

    public struct BeziersControlPointsBuildInput
    {
        public Vector3 initialPosition;
        public Vector3 targetPosition;
        public Vector3 normal;
        public BeziersControlPointsShape BeziersControlPointsShape;
        public float Speed;

        public BeziersControlPointsBuildInput(Vector3 initialPosition, Vector3 targetPosition, Vector3 normal, BeziersControlPointsShape beziersControlPointsShape, float speed)
        {
            this.initialPosition = initialPosition;
            this.targetPosition = targetPosition;
            this.normal = normal;
            BeziersControlPointsShape = beziersControlPointsShape;
            this.Speed = speed;
        }
    }
}