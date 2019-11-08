using UnityEngine;

namespace CoreGame
{
    public struct ExtendedBounds
    {
        private Bounds bounds;

        public Bounds Bounds
        {
            get => bounds;
        }

        public Vector3 SideDistances
        {
            get => sideDistances;
        }

        private bool created;

        public ExtendedBounds(Bounds bounds)
        {
            this.bounds = bounds;
            this.sideDistances = new Vector3(Mathf.Abs(this.bounds.max.x - this.bounds.min.x),
                Mathf.Abs(this.bounds.max.y - this.bounds.min.y),
                Mathf.Abs(this.bounds.max.z - this.bounds.min.z));
            this.created = true;
        }

        private Vector3 sideDistances;

        public bool IsNull()
        {
            return !this.created;
        }
    }

    public class BoundsHelper
    {
        public static ExtendedBounds GetAverageRendererBounds(Renderer[] renderers)
        {
            if (renderers != null && renderers.Length > 0)
            {
                Bounds averageBound = renderers[0].bounds;
                foreach (var renderer in renderers)
                {
                    averageBound.Encapsulate(renderer.bounds);
                }

                return new ExtendedBounds(averageBound);
            }
            else
            {
                return default;
            }
        }
    }
}