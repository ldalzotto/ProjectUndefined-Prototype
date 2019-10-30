using UnityEngine;
using UnityEngine.Rendering;

namespace VisualFeedback
{
    public class DottedLineGameObject
    {
        public GameObject AssociatedGameObject { get; private set; }
        private MeshRenderer MeshRenderer;
        private MeshFilter MeshFilter;

        public DottedLineGameObject(Shader RangeRederingShader, DottedLineInherentData DottedLineInherentData)
        {
            this.AssociatedGameObject = new GameObject("DottedLine");
            this.AssociatedGameObject.transform.parent = DottedLineManager.Get().DottedLineContainer.transform;

            this.AssociatedGameObject.transform.position = Vector3.zero;
            this.AssociatedGameObject.transform.localScale = Vector3.one;
            this.AssociatedGameObject.transform.rotation = Quaternion.identity;

            this.MeshFilter = this.AssociatedGameObject.AddComponent<MeshFilter>();
            this.MeshFilter.mesh = new Mesh();

            this.MeshRenderer = this.AssociatedGameObject.AddComponent<MeshRenderer>();
            this.MeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            this.MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
            this.MeshRenderer.material = new Material(RangeRederingShader);

            this.MeshRenderer.material.SetColor("_BaseColor", DottedLineInherentData.BaseColor);
            this.MeshRenderer.material.SetColor("_MovingColor", DottedLineInherentData.MovingColor);
            this.MeshRenderer.material.SetFloat("_MovingWidth", DottedLineInherentData.MovingWidth);
        }

        public void SetColorPointPosition(Vector3 ColorPointWorldPosition)
        {
            this.MeshRenderer.material.SetVector("_ColorPointPosition", ColorPointWorldPosition);
        }

        public int GetInstanceID()
        {
            return this.AssociatedGameObject.GetInstanceID();
        }

        public void ClearMesh()
        {
            this.MeshFilter.mesh = new Mesh();
        }

        public Mesh GetMesh()
        {
            return this.MeshFilter.mesh;
        }
    }
}