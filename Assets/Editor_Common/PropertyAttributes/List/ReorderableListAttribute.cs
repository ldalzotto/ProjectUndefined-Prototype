using UnityEngine;
using System.Collections;

public class ReorderableListAttribute : PropertyAttribute
{
    public bool DisplayLineLabel = true;

    public ReorderableListAttribute(bool displayLineLabel = true)
    {
        DisplayLineLabel = displayLineLabel;
    }
}
