using System;
using ICSharpCode.NRefactory.Ast;
using Unity.Collections;
using UnityEngine;

namespace GeometryIntersection
{
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