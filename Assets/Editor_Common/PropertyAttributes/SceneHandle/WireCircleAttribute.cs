using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class WireCircleAttribute : AbstractSceneHandleAttribute
{
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class WireCircleWorldAttribute : AbstractSceneHandleAttribute
{
    public bool UseTransform = false;
    public string PositionFieldName;
    public string RadiusFieldName = string.Empty;
    public float Radius = 1f;
}