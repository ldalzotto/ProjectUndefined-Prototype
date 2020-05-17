using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class WireArcAttribute : AbstractSceneHandleAttribute
{
    public float Radius = 5f;
}
