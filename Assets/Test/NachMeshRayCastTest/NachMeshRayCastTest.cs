using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NachMeshRayCastTest : MonoBehaviour
{

    public Transform Source;
    public Transform Target;

    private NavMeshHit NavMeshHit;

    // Update is called once per frame
    void Update()
    {
        NavMesh.Raycast(this.Source.transform.position, this.Target.transform.position, out NavMeshHit hit, NavMesh.AllAreas);
        this.NavMeshHit = hit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.clear;
        Gizmos.DrawWireSphere(this.Source.position, 0.5f);
        Gizmos.DrawWireSphere(this.Target.position, 0.5f);
        
        Gizmos.color = this.NavMeshHit.hit? Color.green : Color.red;
        Gizmos.DrawWireSphere(this.NavMeshHit.position, 1f);
    }
}
