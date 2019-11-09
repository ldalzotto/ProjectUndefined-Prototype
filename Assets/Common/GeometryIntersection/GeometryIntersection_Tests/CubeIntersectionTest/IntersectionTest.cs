using GeometryIntersection;
using UnityEngine;
using UnityEngine.Profiling;

#if UNITY_EDITOR
public class IntersectionTest : MonoBehaviour
{
    public BoxCollider B1;
    public BoxCollider B2;

    private bool Intersect;
    private IGeometryIntersectionJobSingleCalculation BoxIntersection;
    private BoxDefinitionManaged BoxDefinitionManaged1;
    private BoxDefinitionManaged BoxDefinitionManaged2;

    private void Start()
    {
        this.BoxDefinitionManaged1 = new BoxDefinitionManaged(this.B1);
        this.BoxDefinitionManaged2 = new BoxDefinitionManaged(this.B2);
        this.BoxIntersection = new GeometryIntersectionJobSingleCalculation(this.BoxDefinitionManaged1, this.BoxDefinitionManaged2);
        GeometryIntersectionJobManager.Get().Add(this.BoxIntersection);
    }

    private void Update()
    {
        Profiler.BeginSample("IntersectionTest");
        this.Intersect = false;

        //   this.Intersect = Intersection.GeometryGeometryIntersection(Intersection.FromBoxDefinition(new BoxDefinition(this.B1)), Intersection.FromBoxDefinition(new BoxDefinition(this.B2)));

        foreach (var f1s in this.BoxDefinitionManaged1.GetFaces())
        {
            foreach (var fs2 in this.BoxDefinitionManaged2.GetFaces())
            {
                if (Intersection.GeometryFaceIntersection(f1s, fs2))
                {
                    this.Intersect = true;
                    break;
                }
            }

            if (this.Intersect)
            {
                break;
            }
        }

        GeometryIntersectionJobManager.Get().Tick(Time.deltaTime);
        GeometryIntersectionJobManager.Get().WaitForJobHandle();
        this.Intersect = this.BoxIntersection.Intersected();

        Profiler.EndSample();
    }

    private void LateUpdate()
    {
        GeometryIntersectionJobManager.Get().LateTick();
    }

    private void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;

        if (this.Intersect)
        {
            Gizmos.color = Color.green;
        }

        Gizmos.color = this.Intersect ? Color.green : Color.red;
        Gizmos.DrawWireSphere(this.B1.transform.position, 0.5f);


        Gizmos.color = oldColor;
    }
}

#endif