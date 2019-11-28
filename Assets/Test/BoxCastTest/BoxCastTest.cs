using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

public class BoxCastTest : MonoBehaviour
{
    public BoxCollider BoxCollider;

    private GameObject Ref;

    private void Start()
    {
        this.Ref = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        var localToWorld = this.BoxCollider.transform.localToWorldMatrix;

        var insideColliders = this.BoxOverlap(this.BoxCollider);
        if (insideColliders != null && insideColliders.Length > 0)
        {
            for (var i = 0; i < insideColliders.Length; i++)
            {
                Debug.Log(insideColliders[i].name);
            }
        }
    }

    private Collider[] BoxOverlap(BoxCollider boxCollider)
    {
        ExtractBoxColliderWorldPointsV2(new BoxDefinition(boxCollider), out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
        var worldCenter = (C1 + C2 + C3 + C4 + C5 + C6 + C7 + C8) / 8f;
        var worldSize = new Vector3(Vector3.Distance(C1, C2), Vector3.Distance(C2, C3), Vector3.Distance(C1, C5));
        var worldCenterRotation = Quaternion.LookRotation((C5 - C1).normalized, (C2 - C3).normalized);

        this.Ref.transform.position = worldCenter;
        this.Ref.transform.rotation = worldCenterRotation;
        this.Ref.transform.localScale = worldSize;
        
        return Physics.OverlapBox(worldCenter,
            worldSize * 0.5f,
            worldCenterRotation);
    }


    [Serializable]
    public struct BoxDefinition
    {
        public Vector3 LocalCenter;
        public Vector3 LocalSize;
        public Matrix4x4 LocalToWorld;

        public BoxDefinition(BoxCollider BoxCollider)
        {
            this.LocalCenter = BoxCollider.center;
            this.LocalSize = BoxCollider.size;
            this.LocalToWorld = BoxCollider.transform.localToWorldMatrix;
        }
    }

    public static void ExtractBoxColliderWorldPointsV2(BoxDefinition BoxDefinition, out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8)
    {
        ExtractBoxColliderLocalPoints(BoxDefinition.LocalCenter, BoxDefinition.LocalSize, out Vector3 lC1, out Vector3 lC2, out Vector3 lC3, out Vector3 lC4, out Vector3 lC5,
            out Vector3 lC6, out Vector3 lC7, out Vector3 lC8);

        var boxLocalToWorld = BoxDefinition.LocalToWorld;

        C1 = boxLocalToWorld.MultiplyPoint(lC1);
        C2 = boxLocalToWorld.MultiplyPoint(lC2);
        C3 = boxLocalToWorld.MultiplyPoint(lC3);
        C4 = boxLocalToWorld.MultiplyPoint(lC4);
        C5 = boxLocalToWorld.MultiplyPoint(lC5);
        C6 = boxLocalToWorld.MultiplyPoint(lC6);
        C7 = boxLocalToWorld.MultiplyPoint(lC7);
        C8 = boxLocalToWorld.MultiplyPoint(lC8);
    }

    public static void ExtractBoxColliderLocalPoints(Vector3 localCenter, Vector3 localSize, out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8)
    {
        Vector3 diagDirection = Vector3.zero;


        diagDirection = diagDirection.SetVector(-localSize.x, localSize.y, -localSize.z) * 0.5f;
        C1 = localCenter + diagDirection;

        diagDirection = diagDirection.SetVector(localSize.x, localSize.y, -localSize.z) * 0.5f;
        C2 = localCenter + diagDirection;

        diagDirection = diagDirection.SetVector(localSize.x, -localSize.y, -localSize.z) * 0.5f;
        C3 = localCenter + diagDirection;

        diagDirection = diagDirection.SetVector(-localSize.x, -localSize.y, -localSize.z) * 0.5f;
        C4 = localCenter + diagDirection;

        diagDirection = diagDirection.SetVector(-localSize.x, localSize.y, localSize.z) * 0.5f;
        C5 = localCenter + diagDirection;

        diagDirection = diagDirection.SetVector(localSize.x, localSize.y, localSize.z) * 0.5f;
        C6 = localCenter + diagDirection;

        diagDirection = diagDirection.SetVector(localSize.x, -localSize.y, localSize.z) * 0.5f;
        C7 = localCenter + diagDirection;

        diagDirection = diagDirection.SetVector(-localSize.x, -localSize.y, localSize.z) * 0.5f;
        C8 = localCenter + diagDirection;
    }
}