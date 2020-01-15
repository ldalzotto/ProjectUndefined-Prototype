using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Skill
{
    /// <summary>
    /// This <see cref="BaseMeshEffect"/> will calculates the undistorted UV in the range of [0,1] from local position and push it to uv1.
    /// </summary>
    public class PushUndistortedUVtoUV1 : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            int vertCount = vh.currentVertCount;

            var vert = new UIVertex();

            vh.PopulateUIVertex(ref vert, 0);

            float minX = vert.position.x;
            float maxX = vert.position.x;
            float minY = vert.position.y;
            float maxY = vert.position.y;

            for (int v = 1; v < vertCount; v++)
            {
                vh.PopulateUIVertex(ref vert, v);
                minX = Mathf.Min(minX, vert.position.x);
                maxX = Mathf.Max(maxX, vert.position.x);
                minY = Mathf.Min(minY, vert.position.y);
                maxY = Mathf.Max(maxY, vert.position.y);
            }

            for (int v = 0; v < vertCount; v++)
            {
                vh.PopulateUIVertex(ref vert, v);

                vert.uv1 = new Vector2((vert.position.x - minX) / (maxX - minX), (vert.position.y - minY) / (maxY - minY));
                vh.SetUIVertex(vert, v);
            }
        }
    }
}