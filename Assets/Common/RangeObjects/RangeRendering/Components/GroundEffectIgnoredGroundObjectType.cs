using System.Linq;
using UnityEngine;

namespace RangeObjects
{
    public class GroundEffectIgnoredGroundObjectType : MonoBehaviour
    {
        private Mesh groundEffectMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public Mesh GroundEffectMesh => groundEffectMesh;

        public MeshRenderer MeshRenderer => meshRenderer;

        public MeshFilter MeshFilter => meshFilter;

        public void Init()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            groundEffectMesh = new Mesh();
            groundEffectMesh.SetVertices(meshFilter.mesh.vertices.ToList());
            groundEffectMesh.SetTriangles(meshFilter.mesh.triangles, 0);
        }
    }
}