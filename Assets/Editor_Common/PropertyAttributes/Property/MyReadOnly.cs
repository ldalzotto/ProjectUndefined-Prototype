using UnityEngine;
using UnityEditor;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class MyReadOnly : PropertyAttribute
{
}