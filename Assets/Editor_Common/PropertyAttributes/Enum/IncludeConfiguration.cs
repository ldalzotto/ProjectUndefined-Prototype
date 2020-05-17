using System;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class IncludeConfiguration : PropertyAttribute
{
    public Type ConfigurationType;

    public IncludeConfiguration(Type configurationType)
    {
        this.ConfigurationType = configurationType;
    }

}

