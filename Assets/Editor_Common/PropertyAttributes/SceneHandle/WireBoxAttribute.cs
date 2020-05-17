using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class WireBoxAttribute : AbstractSceneHandleAttribute
{
    public string CenterFieldName;
    public string SizeFieldName;
}
