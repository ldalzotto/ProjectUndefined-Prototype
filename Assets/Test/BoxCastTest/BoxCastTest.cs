using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

public class BoxCastTest : MonoBehaviour
{
    public BoxCollider BoxCollider;


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
        var localToWorld = Matrix4x4.TRS(boxCollider.transform.position, Quaternion.identity, boxCollider.transform.lossyScale);
        var insideColliders = Physics.OverlapBox(boxCollider.transform.position + boxCollider.center,
            boxCollider.transform.lossyScale.Mul(boxCollider.size) * 0.5f,
            boxCollider.transform.rotation);
        return insideColliders;
    }

    private void OnDrawGizmos()
    {
        GizmoHelper.DrawBox(this.BoxCollider.transform.localToWorldMatrix, BoxCollider.center, new Bounds(BoxCollider.center, BoxCollider.size), Color.blue, "TAMERE", GUIStyle.none);
        //  Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}