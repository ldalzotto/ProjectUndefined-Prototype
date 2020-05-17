using UnityEngine;
using System.Collections;
using System;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class DrawDefinitionAttribute : PropertyAttribute
{
    public Type ConfigurationType;
}
