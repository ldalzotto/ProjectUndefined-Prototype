using GeometryIntersection;
using UnityEngine;

[ExecuteInEditMode]
public class IntersectionTest : MonoBehaviour
{
    public Transform C1;
    public Transform C2;
    public Transform C3;
    public Transform C4;

    public Transform P1;
    public Transform P2;

    private Vector3 I;
    private Vector3 center;
    Vector3 normal12;
    Vector3 normal23;
    Vector3 normal34;

    private Vector3 normal41;
    public BoxCollider B1;
    public BoxCollider B2;


    private bool Intersect;
    private SingleFacePosition[] B1Faces;

    private void Update()
    {
        this.B1Faces = Intersection.FromBoxDefinition(new BoxDefinition(this.B1));
        var B2Faces = Intersection.FromBoxDefinition(new BoxDefinition(this.B2));
        this.Intersect = Intersection.GeometryGeometryIntersection(this.B1Faces, B2Faces);
    }

    private void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;

        if (this.Intersect)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(this.I, 0.1f);
        }


        if (this.B1 != null && this.B1Faces != null)
        {
            Gizmos.color = this.Intersect ? Color.green : Color.red;
            foreach (var facePosition in B1Faces)
            {
                Gizmos.DrawLine(facePosition.FC1, facePosition.FC2);
                Gizmos.DrawLine(facePosition.FC2, facePosition.FC3);
                Gizmos.DrawLine(facePosition.FC3, facePosition.FC4);
                Gizmos.DrawLine(facePosition.FC4, facePosition.FC1);
            }

            Gizmos.DrawWireSphere(this.B1.transform.position, 0.5f);
        }

        Gizmos.color = oldColor;
    }
}