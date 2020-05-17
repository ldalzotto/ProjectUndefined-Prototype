using UnityEngine;
using System.Collections;
using System;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class DrawConfigurationAttribute : PropertyAttribute
{
    public Type ConfigurationType;
}
