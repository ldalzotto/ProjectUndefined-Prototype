using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NachMeshRayCastTest : MonoBehaviour
{
    public Transform Source;
    public Transform Target;
    public string DynamicSceneName;
    public bool Load;
    public bool Unload;

    private NavMeshHit NavMeshHit;
    private bool Dashable;

    private void Start()
    {
        //   SceneManager.LoadScene(DynamicSceneName, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        if (Load)
        {
            SceneManager.LoadScene(DynamicSceneName, LoadSceneMode.Additive);
            Load = false;
        }

        if (Unload)
        {
            SceneManager.UnloadScene(DynamicSceneName);
            Unload = false;
        }

        NavMesh.Raycast(this.Source.transform.position, this.Target.transform.position, out NavMeshHit hit, 1 << NavMesh.GetAreaFromName("DashableSpace"));
        this.NavMeshHit = hit;
        this.Dashable = NavMesh.SamplePosition(hit.position, out NavMeshHit hit2, 0.1f, 1 << 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.clear;
        Gizmos.DrawWireSphere(this.Source.position, 0.5f);
        Gizmos.DrawWireSphere(this.Target.position, 0.5f);

        Gizmos.color = this.Dashable ? Color.green : Color.red;
        Gizmos.DrawWireSphere(this.NavMeshHit.position, 1f);
    }
}