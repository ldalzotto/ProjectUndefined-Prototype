using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class Inline : PropertyAttribute
{
    public bool CreateAtSameLevelIfAbsent = false;
    public string FileName;

    public Inline(bool createAtSameLevelIfAbsent = false)
    {
        CreateAtSameLevelIfAbsent = createAtSameLevelIfAbsent;
    }
}
