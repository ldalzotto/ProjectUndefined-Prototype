using UnityEngine;

namespace CoreGame
{
    public struct Segment
    {
        public Vector3 Source;
        public Vector3 Target;
        public float Distance;

        public Segment(Vector3 source, Vector3 target)
        {
            Source = source;
            Target = target;
            Distance = Vector3.Distance(Source, Target);
        }
    }
}