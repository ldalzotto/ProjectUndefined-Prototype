using System;
using Unity.Collections;
using UnityEngine;

namespace GeometryIntersection
{
    /// <summary>
    /// A <see cref="VisibilityProbe"/> is a set a local points <see cref="LocalPoints"/> that are used by Sight detection algorithm to check of the associated
    /// interactive object is contained in the sight volume or not.
    /// These local points are discrete representation of the volume that will be used by the algorithm. Is order for an interactive object to be visible, at least
    /// one of these points must be visible.
    /// The more points there are, the more accurate is the calculation, but the less performant it is.
    /// This method has been preferred over 3D shape intersection because I wasn't aware of sat algorithm. And the sat algorithm is probably cheaper than probe checking against a 3D geometry.
    /// </summary>
    [Serializable]
    public struct VisibilityProbe
    {
        public Vector3[] LocalPoints;

        public static VisibilityProbe Allocate(int NumberOfPoints)
        {
            VisibilityProbe VisibilityProbe = new VisibilityProbe();
            VisibilityProbe.LocalPoints = new Vector3[NumberOfPoints];
            return VisibilityProbe;
        }

        public Vector3 this[int i]
        {
            get { return this.LocalPoints[i]; }
            set { this.LocalPoints[i] = value; }
        }

        public int GetSize()
        {
            if (this.LocalPoints != null)
            {
                return this.LocalPoints.Length;
            }

            return 0;
        }
    }
}