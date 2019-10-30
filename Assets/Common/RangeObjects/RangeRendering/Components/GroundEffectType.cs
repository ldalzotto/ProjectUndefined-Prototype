using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RangeObjects
{
    public class GroundEffectType : MonoBehaviour
    {
        private List<GroundEffectIgnoredGroundObjectType> associatedGroundEffectIgnoredGroundObjectType;
        private List<CombineInstance> groundEffectCombinedInstances;
        private Mesh groundEffectMesh;

        private bool hasBeenInit;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public MeshRenderer MeshRenderer => meshRenderer;

        public MeshFilter MeshFilter => meshFilter;

        public Mesh GroundEffectMesh => groundEffectMesh;

        public List<GroundEffectIgnoredGroundObjectType> AssociatedGroundEffectIgnoredGroundObjectType => associatedGroundEffectIgnoredGroundObjectType;

        public void Init()
        {
            if (!hasBeenInit)
            {
                meshRenderer = GetComponent<MeshRenderer>();
                meshFilter = GetComponent<MeshFilter>();
                groundEffectMesh = new Mesh();
                groundEffectMesh.SetVertices(meshFilter.mesh.vertices.ToList());
                groundEffectMesh.SetTriangles(meshFilter.mesh.triangles, 0);

                var childsGroundEffectIgnoredGroundObjectType = GetComponentsInChildren<GroundEffectIgnoredGroundObjectType>();

                if (childsGroundEffectIgnoredGroundObjectType != null)
                    associatedGroundEffectIgnoredGroundObjectType = childsGroundEffectIgnoredGroundObjectType.ToList();
                else
                    associatedGroundEffectIgnoredGroundObjectType = new List<GroundEffectIgnoredGroundObjectType>();

                foreach (var GroundEffectIgnoredGroundObjectType in associatedGroundEffectIgnoredGroundObjectType)
                    GroundEffectIgnoredGroundObjectType.Init();

                groundEffectCombinedInstances = new List<CombineInstance>();
                groundEffectCombinedInstances.Add(new CombineInstance() {mesh = groundEffectMesh, transform = transform.localToWorldMatrix});
                groundEffectCombinedInstances.AddRange(associatedGroundEffectIgnoredGroundObjectType.ConvertAll(GroundEffectIgnoredGroundObjectType => new CombineInstance() {mesh = GroundEffectIgnoredGroundObjectType.GroundEffectMesh, transform = GroundEffectIgnoredGroundObjectType.transform.localToWorldMatrix}));

                hasBeenInit = true;
            }
        }

        public List<CombineInstance> GetCombineInstances()
        {
            return groundEffectCombinedInstances;
        }
    }
}