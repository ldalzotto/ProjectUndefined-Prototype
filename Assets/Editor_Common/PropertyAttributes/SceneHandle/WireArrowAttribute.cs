using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class WireArrowAttribute : AbstractSceneHandleAttribute
{
    public Vector3 Source;
    public string SourceFieldName;

    public Vector3 Target;
    public string TargetFieldName;

    public float ArrowSemiAngle = 25f;
    public float ArrowLength = 3f;
}