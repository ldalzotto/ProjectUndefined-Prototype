using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class CustomEnum : PropertyAttribute
{
    public bool IsSearchable;
    public Type ConfigurationType;
    public bool OpenToConfiguration;

    public CustomEnum(bool isSearchable = true, Type configurationType = null, bool openToConfiguration = false)
    {
        this.IsSearchable = isSearchable;
        this.ConfigurationType = configurationType;
        this.OpenToConfiguration = openToConfiguration;
    }
}