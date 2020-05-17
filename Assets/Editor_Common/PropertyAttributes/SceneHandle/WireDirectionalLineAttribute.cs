using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class WireDirectionalLineAttribute : AbstractSceneHandleAttribute
{
    public float dX = 0f;
    public float dY = 0f;
    public float dZ = 0f;
}